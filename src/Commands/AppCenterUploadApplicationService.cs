using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AppCenterClient.Commands.Models;
using AppCenterClient.Utils;

namespace AppCenterClient.Commands
{
    public sealed class AppCenterUploadApplicationService : IDisposable
    {
        private const int MaxRetriesCount = 10;
        private const int DelayAfterErrorInterval = 10;
        private readonly HttpClient _httpClient = new HttpClient();

        private readonly Dictionary<string, string> _uploadContentTypes = new Dictionary<string, string>
        {
            {"apk", "application/vnd.android.package-archive"},
            {"aab", "application/vnd.android.package-archive"},
            {"msi", "application/x-msi"},
            {"plist", "application/xml"},
            {"aetx", "application/c-x509-ca-cert"},
            {"cer", "application/pkix-cert"},
            {"xap", "application/x-silverlight-app"},
            {"appx", "application/x-appx"},
            {"appxbundle", "application/x-appxbundle"},
            {"appxupload", "application/x-appxupload"},
            {"appxsym", "application/x-appxupload"},
            {"msix", "application/x-msix"},
            {"msixbundle", "application/x-msixbundle"},
            {" msixupload", "application/x-msixupload"},
            {"msixsym", "application/x-msixupload"}
        };

        public AppCenterUploadApplicationService(string uploadDomain, string token)
        {
            _httpClient.BaseAddress = new Uri(uploadDomain);
            _httpClient.Timeout = TimeSpan.FromMinutes(30);
            _httpClient.DefaultRequestHeaders.Add("X-API-Token", token);
        }

        public Task<SetMetadataUploadResponse> SetMetadata(string fileName, string packageAssetId, string urlEncodedToken)
        {
            var file = new FileInfo(fileName);
            var contentType = GetContentType(file.Extension);
            return _httpClient.AppCenterPostRequest<SetMetadataUploadResponse>($"upload/set_metadata/{packageAssetId}?file_name={file.Name}&file_size={file.Length}&token={urlEncodedToken}&content_type={contentType}");
        }

        public async Task UploadFile(string fileName, int chunkSize, string packageAssetId, string urlEncodedToken)
        {
            var baseUrl = $"upload/upload_chunk/{packageAssetId}?&token={urlEncodedToken}";
            var blockNumber = 1;
            foreach (var chunk in ReadFileChunks(fileName, chunkSize))
            {
                Exception? uploadException = null;
                for (var i = 0; i < MaxRetriesCount; ++i)
                {
                    try
                    {
                        var response = await _httpClient.AppCenterUploadRequest<UploadFileResponse>($"{baseUrl}&block_number={blockNumber}", chunk);
                        if (response.Error)
                        {
                            throw new HttpRequestException($"Upload chunk failed. Error='{response.ErrorCode}', ChunkNumber='{response.ChunkNum}'");
                        }

                        blockNumber++;
                        uploadException = null;
                        break;
                    }
                    catch (Exception e)
                    {
                        uploadException = e;
                        await Task.Delay(TimeSpan.FromSeconds(DelayAfterErrorInterval));
                    }
                }

                if (uploadException != null)
                {
                    throw uploadException;
                }
            }
        }

        public async Task<FinishUploadResponse> FinishUpload(string packageAssetId, string urlEncodedToken)
        {
            var response = await _httpClient.AppCenterPostRequest<FinishUploadResponse>($"upload/finished/{packageAssetId}?token={urlEncodedToken}");
            if (response.Error)
            {
                throw new HttpRequestException($"Finish upload error: ErrorCode='{response.ErrorCode}' State='{response.State}', Message='{response.Message}'");
            }

            return response;
        }


        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private string GetContentType(string fileExt) => _uploadContentTypes.TryGetValue(fileExt.TrimStart('.').ToLowerInvariant(), out var contentType) ? contentType : "application/octet-stream";

        private static IEnumerable<byte[]> ReadFileChunks(string fileName, int chunkSize)
        {
            int bytesRead;
            var buffer = new byte[chunkSize];
            using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (bytesRead >= buffer.Length)
                {
                    yield return buffer;
                }
                else
                {
                    var truncatedBuffer = new byte[bytesRead];
                    Array.Copy(buffer, truncatedBuffer, truncatedBuffer.Length);
                    yield return truncatedBuffer;
                }
            }
        }
    }
}