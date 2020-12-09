using System.Text.Json.Serialization;

namespace AppCenterClient.Commands.Distribute.ReleaseCommand.Models
{
    public class ReleaseAddDistributionGroupResponse
    {
        [JsonPropertyName("v")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("mandatory_update")] public bool MandatoryUpdate { get; set; } = false;
        [JsonPropertyName("provisioning_status_url")] public string? ProvisioningStatusUrl { get; set; }
    }
}