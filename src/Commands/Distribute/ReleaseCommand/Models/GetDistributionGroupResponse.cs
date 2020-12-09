using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand.Models
{
    public class GetDistributionGroupResponse
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("display_name")] public string? DisplayName { get; set; }
        [JsonPropertyName("origin")] public string Origin { get; set; } = string.Empty;
        [JsonPropertyName("is_public")] public bool IsPublic { get; set; } = false;
    }
}