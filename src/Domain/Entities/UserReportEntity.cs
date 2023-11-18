using System.Text.Json.Serialization;
using Domain.Entities.Base;

namespace Domain.Entities;

public class UserReportEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => GetPk();
    [JsonPropertyName("sk")] public string Sk => $"{ReportedId}#{ReporterId}";
    [JsonPropertyName("reporterId")] public string ReporterId { get; set; } = default!;
    [JsonPropertyName("reportedId")] public string ReportedId { get; set; } = default!;
    [JsonPropertyName("reason")] public string? Reason { get; set; }
    [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }

    public static string GetPk()
    {
        return $"userReport";
    }
}