using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Models
{
    public class SetMetadataUploadResponse
    {
        [JsonPropertyName("chunk_size")] public int ChunkSize { get; set; } = 0;
    }
}