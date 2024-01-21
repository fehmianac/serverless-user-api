using System.Text.Json.Serialization;
using Domain.Entities.Base;
using Domain.Enums;
using Domain.Extensions;

namespace Domain.Entities;

public class OtpCodeEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => $"OtpCodes#{Type}";
    [JsonPropertyName("sk")] public string Sk => Code;
    [JsonPropertyName("code")] public string Code { get; set; } = default!;
    [JsonPropertyName("type")] public UniqueKeyType Type { get; set; }
    [JsonPropertyName("userId")] public string UserId { get; set; } = default!;
    [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("expireAt")] public DateTime ExpireAt { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("ttl")] public long Ttl => ExpireAt.ToUnixTimeSeconds();
}