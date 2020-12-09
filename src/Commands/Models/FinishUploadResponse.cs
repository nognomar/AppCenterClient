using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Models
{
    public class FinishUploadResponse
    {
        [JsonPropertyName("error")] public bool Error { get; set; } = false;
        [JsonPropertyName("error_code")] public string ErrorCode { get; set; } = string.Empty;
        [JsonPropertyName("state")] public string State { get; set; } = string.Empty;
        [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    }
}