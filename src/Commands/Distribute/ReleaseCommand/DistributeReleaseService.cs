using System;
using System.Net.Http;
using System.Threading.Tasks;
using AppCenterClient.Commands.Distribute.ReleaseCommand.Models;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand
{
    public class DistributeReleaseService : AppCenterService
    {
        private readonly TimeSpan _waitForReleaseDelay = TimeSpan.FromSeconds(10);

        public DistributeReleaseService(string baseUrl, string token) : base(baseUrl, token)
        {
        }

        public Task<GetReleaseUploadStatusResponse> GetReleaseUploadStatus(string ownerName, string appName, string uploadId) =>
            Get<GetReleaseUploadStatusResponse>($"v0.1/apps/{ownerName}/{appName}/uploads/releases/{uploadId}");

        public Task<UpdateReleaseUploadStatusResponse> UpdateReleaseStatus(string ownerName, string appName, string uploadId, UpdateReleaseUploadStatusRequest request) =>
            Patch<UpdateReleaseUploadStatusResponse>($"v0.1/apps/{ownerName}/{appName}/uploads/releases/{uploadId}", request);

        public Task<CreateReleaseUploadResponse> CreateReleaseUpload(string ownerName, string appName, CreateReleaseUploadRequest request) =>
            Post<CreateReleaseUploadResponse>($"v0.1/apps/{ownerName}/{appName}/uploads/releases", request);

        public Task<GetDistributionGroupResponse> GetDistributionGroup(string ownerName, string appName, string distributionGroupName) =>
            Get<GetDistributionGroupResponse>($"v0.1/apps/{ownerName}/{appName}/distribution_groups/{distributionGroupName}");

        public Task<ReleaseAddDistributionGroupResponse> AddDistributionGroupToRelease(string ownerName, string appName, long releaseId, ReleaseAddDistributionGroupRequest request) =>
            Post<ReleaseAddDistributionGroupResponse>($"v0.1/apps/{ownerName}/{appName}/releases/{releaseId}/groups", request);

        public async Task<GetReleaseUploadStatusResponse> WaitForReleaseId(string ownerName, string appName, string uploadId)
        {
            while (true)
            {
                var response = await GetReleaseUploadStatus(ownerName, appName, uploadId);
                switch (response.UploadStatus)
                {
                    case "readyToBePublished":
                        return response;
                    case "malwareDetected":
                    case "error":
                        throw new HttpRequestException($"Can't get release id: UploadStatus='{response.UploadStatus}', Error='{response.ErrorDetails}'");
                    default:
                        await Task.Delay(_waitForReleaseDelay);
                        break;
                }
            }
        }
    }
}