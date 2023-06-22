using System.Text.Json.Serialization;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class UniqueKeyEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => $"uniqueData#{Type}";

    [JsonPropertyName("sk")] public string Sk => Value;
    [JsonPropertyName("value")] public string Value { get; set; } = default!;
    [JsonPropertyName("type")] public UniqueKeyType Type { get; set; }
    [JsonPropertyName("userId")] public string UserId { get; set; } = default!;
    [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}