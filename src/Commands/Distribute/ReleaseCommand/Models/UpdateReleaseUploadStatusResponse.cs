using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand.Models
{
    public class UpdateReleaseUploadStatusResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("upload_status")] public string UploadStatus { get; set; } = string.Empty;
    }
}