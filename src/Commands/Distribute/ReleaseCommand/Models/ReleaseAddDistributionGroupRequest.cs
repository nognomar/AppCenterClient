using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand.Models
{
    public class ReleaseAddDistributionGroupRequest
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("mandatory_update")] public bool MandatoryUpdate { get; set; } = false;
        [JsonPropertyName("notify_testers")] public bool NotifyTesters { get; set; } = false;
    }
}