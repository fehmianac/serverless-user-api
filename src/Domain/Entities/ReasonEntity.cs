using System.Text.Json.Serialization;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class ReasonEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => GetPk(Type);
    [JsonPropertyName("sk")] public string Sk => UserId;
    [JsonPropertyName("type")] public ReasonType Type { get; set; }
    [JsonPropertyName("reasonId")] public string ReasonId { get; set; } = default!;
    [JsonPropertyName("userId")] public string UserId { get; set; } = default!;
    [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }

    public static string GetPk(ReasonType type)
    {
        return "reasons#" + type;
    }
}