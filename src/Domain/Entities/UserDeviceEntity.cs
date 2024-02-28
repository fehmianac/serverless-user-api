using System.Text.Json.Serialization;
using Domain.Entities.Base;

namespace Domain.Entities;

public class UserDeviceEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => $"userDevices#{UserId}";
    [JsonPropertyName("sk")] public string Sk => $"{Id}";
    [JsonPropertyName("id")] public string Id { get; set; } = default!;
    [JsonPropertyName("userId")] public string UserId { get; set; } = default!;
    [JsonPropertyName("platform")] public string? Platform { get; set; }
    [JsonPropertyName("additionalData")] public Dictionary<string, string> AdditionalData { get; set; } = new();
}