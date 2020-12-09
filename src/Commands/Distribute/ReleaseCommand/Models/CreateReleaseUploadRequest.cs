using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand.Models
{
    public class CreateReleaseUploadRequest
    {
        [JsonPropertyName("build_version")] public string BuildVersion { get; set; } = string.Empty;
        [JsonPropertyName("build_number")] public string BuildNumber { get; set; } = string.Empty;
    }
}