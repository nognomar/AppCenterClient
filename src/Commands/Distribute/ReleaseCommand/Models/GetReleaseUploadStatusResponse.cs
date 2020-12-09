using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand.Models
{
    public class GetReleaseUploadStatusResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("upload_status")] public string UploadStatus { get; set; } = string.Empty;
        [JsonPropertyName("error_details")] public string? ErrorDetails { get; set; }

        [JsonPropertyName("release_distinct_id")]
        public long? ReleaseDistinctId { get; set; }

        [JsonPropertyName("release_url")] public string? ReleaseUrl { get; set; }
    }
}