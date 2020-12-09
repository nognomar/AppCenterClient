using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Models
{
    public class UploadFileResponse
    {
        [JsonPropertyName("error")] public bool Error { get; set; } = false;
        [JsonPropertyName("error_code")] public string ErrorCode { get; set; } = string.Empty;
        [JsonPropertyName("chunk_num")] public int ChunkNum { get; set; } = 0;
    }
}