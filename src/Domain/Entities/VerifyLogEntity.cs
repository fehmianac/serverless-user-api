using System.Text.Json.Serialization;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class VerifyLogEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => $"verifiedLogs#{UserId}";

    [JsonPropertyName("sk")] public string Sk => Type.ToString("D");
    [JsonPropertyName("type")] public UniqueKeyType Type { get; set; }
    [JsonPropertyName("userId")] public string UserId { get; set; } = default!;
    [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}