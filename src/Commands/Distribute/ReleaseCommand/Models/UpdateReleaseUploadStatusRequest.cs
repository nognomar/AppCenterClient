using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand.Models
{
    public class UpdateReleaseUploadStatusRequest
    {
        [JsonPropertyName("upload_status")] public string UploadStatus { get; set; } = string.Empty;
    }
}