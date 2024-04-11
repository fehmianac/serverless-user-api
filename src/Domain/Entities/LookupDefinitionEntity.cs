using System.Text.Json.Serialization;
using Domain.Dto;
using Domain.Entities.Base;

namespace Domain.Entities;

public class LookupDefinitionEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => GetPk();

    [JsonPropertyName("sk")] public string Sk => $"{Type}#{Id}";
    [JsonPropertyName("id")] public string Id { get; set; } = default!;
    [JsonPropertyName("type")] public string Type { get; set; } = default!;
    [JsonPropertyName("name")] public string Name { get; set; } = default!;
    [JsonPropertyName("translations")] public List<TranslationDto> Translations { get; set; } = new();
    
    [JsonPropertyName("rank")] public int Rank { get; set; }

    public static string GetPk()
    {
        return "Lookups";
    }
}