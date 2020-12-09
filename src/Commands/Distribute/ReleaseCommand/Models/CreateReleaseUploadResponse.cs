using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand.Models
{
    public class CreateReleaseUploadResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("upload_domain")] public string UploadDomain { get; set; } = string.Empty;
        [JsonPropertyName("token")] public string Token { get; set; } = string.Empty;

        [JsonPropertyName("url_encoded_token")]
        public string UrlEncodedToken { get; set; } = string.Empty;

        [JsonPropertyName("package_asset_id")] public string PackageAssetId { get; set; } = string.Empty;
    }
}