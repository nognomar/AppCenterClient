using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCenterClient.Attributes;
using AppCenterClient.Commands.Distribute.ReleaseCommand.Models;
using AppCenterClient.Commands.Models;
using CommandLine;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand
{
    [AppCenterCommand("distribute", "release", Help = "Upload & Release application to distributors")]
    public class DistributeReleaseCommand : AppCenterCommand
    {
        [Option('o', "owner", Required = true, HelpText = "AppCenter application owner")]
        public string Owner { get; set; } = string.Empty;

        [Option('n', "app-name", Required = true, HelpText = "AppCenter application name")]
        public string AppName { get; set; } = string.Empty;

        [Option('f', "app-file", Required = true, HelpText = "Uploading file path.")]
        public string AppFile { get; set; } = string.Empty;

        [Option('v', "build-version", Required = true, HelpText = "AppCenter application build version")]
        public string BuildVersion { get; set; } = string.Empty;

        [Option("build-number", Required = false, Default = "", HelpText = "AppCenter application build number")]
        public string BuildNumber { get; set; } = string.Empty;

        [Option('g', "distribution-group", Required = true, Separator = ',', HelpText = "Comma-separated list of release groups")]
        public IEnumerable<string> DistributionGroups { get; set; } = new List<string>();

        [Option("notify-testers", Required = false, Default = false, HelpText = "Notify testers about uploaded release")]
        public bool NotifyTesters { get; set; } = false;

        protected override async Task RunInternal()
        {
            Console.WriteLine("Distribute App Release started.");
            using var service = GetService<DistributeReleaseService>();

            var releaseUpload = await CreateReleaseUpload(service);

            await UploadApp(releaseUpload);

            var updateReleaseUpload = await UpdateUploadReleaseStatus(service, releaseUpload.Id);
            var releaseId = await WaitForReleaseId(service, updateReleaseUpload.Id);
            await AddDistributionGroupsToRelease(service, releaseId);

            Console.WriteLine("Distribute App Release finished.");
        }

        private async Task<CreateReleaseUploadResponse> CreateReleaseUpload(DistributeReleaseService service)
        {
            Console.WriteLine("Create Release Upload.");
            var response = await service.CreateReleaseUpload(Owner, AppName, new CreateReleaseUploadRequest
            {
                BuildVersion = BuildVersion,
                BuildNumber = BuildNumber
            });

            Console.WriteLine($"Release Upload created: UploadId='{response.Id}', PackageAssetId='{response.PackageAssetId}'");
            return response;
        }

        private async Task<UpdateReleaseUploadStatusResponse> UpdateUploadReleaseStatus(DistributeReleaseService service, string uploadId)
        {
            Console.WriteLine("Update Release Upload Status.");
            var response = await service.UpdateReleaseStatus(Owner, AppName, uploadId, new UpdateReleaseUploadStatusRequest
            {
                UploadStatus = "uploadFinished"
            });
            Console.WriteLine($"Release Upload Status updated: Status='{response.UploadStatus}'");
            return response;
        }

        private async Task<long> WaitForReleaseId(DistributeReleaseService service, string uploadId)
        {
            Console.WriteLine("Wait For Release Id.");
            var response = await service.WaitForReleaseId(Owner, AppName, uploadId);
            Console.WriteLine($"Release Id: {response.ReleaseDistinctId}");
            return response.ReleaseDistinctId!.Value;
        }

        private async Task AddDistributionGroupsToRelease(DistributeReleaseService service, long releaseId)
        {
            Console.WriteLine("Distribute release.");
            foreach (var distributionGroup in DistributionGroups)
            {
                var group = await service.GetDistributionGroup(Owner, AppName, distributionGroup);
                await service.AddDistributionGroupToRelease(Owner, AppName, releaseId, new ReleaseAddDistributionGroupRequest
                {
                    Id = group.Id,
                    MandatoryUpdate = false,
                    NotifyTesters = NotifyTesters
                });
            }

            Console.WriteLine("Distribute release finished.");
        }

        private async Task UploadApp(CreateReleaseUploadResponse releaseUpload)
        {
            Console.WriteLine("File Uploading started.");
            using var service = GetUploadApplicationService(releaseUpload.UploadDomain);
            var uploadMetadata = await SetMetadata(service, releaseUpload.PackageAssetId, releaseUpload.UrlEncodedToken);
            await UploadFile(service, uploadMetadata.ChunkSize, releaseUpload.PackageAssetId, releaseUpload.UrlEncodedToken);
            await FinishUpload(service, releaseUpload.PackageAssetId, releaseUpload.UrlEncodedToken);
            Console.WriteLine("File Uploading finished.");
        }

        private async Task<SetMetadataUploadResponse> SetMetadata(AppCenterUploadApplicationService service, string packageAssetId, string urlEncodedToken)
        {
            Console.WriteLine("Set Metadata");
            var response = await service.SetMetadata(AppFile, packageAssetId, urlEncodedToken);
            Console.WriteLine($"Metadata setup: UploadFileChunkSize='{response.ChunkSize}'");
            return response;
        }

        private async Task UploadFile(AppCenterUploadApplicationService service, int chunkSize, string packageAssetId, string urlEncodedToken)
        {
            Console.WriteLine("Upload app file.");
            await service.UploadFile(AppFile, chunkSize, packageAssetId, urlEncodedToken);
            Console.WriteLine("App file uploaded.");
        }

        private static async Task FinishUpload(AppCenterUploadApplicationService service, string packageAssetId, string urlEncodedToken)
        {
            Console.WriteLine("Finish upload.");
            var response = await service.FinishUpload(packageAssetId, urlEncodedToken);
            Console.WriteLine($"Finish upload result: State='{response.State}', Message='{response.Message}'");
        }
    }
}