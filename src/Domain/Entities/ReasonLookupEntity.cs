using System.Text.Json.Serialization;
using Domain.Dto;
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class ReasonLookupEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => GetPk(Type);
    [JsonPropertyName("sk")] public string Sk => Id;

    [JsonPropertyName("id")] public string Id { get; set; } = default!;
    [JsonPropertyName("type")] public ReasonType Type { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = default!;
    
    [JsonPropertyName("rank")] public int Rank { get; set; }
    [JsonPropertyName("translations")] public List<TranslationDto> Translations { get; set; } = new();

    public static string GetPk(ReasonType type)
    {
        return $"reasonsLookup#{type}";
    }
}