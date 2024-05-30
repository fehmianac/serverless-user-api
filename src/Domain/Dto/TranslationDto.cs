using System.Text.Json.Serialization;

namespace Domain.Dto;

public class TranslationDto
{
    [JsonPropertyName("culture")] public string Culture { get; set; } = default!;
    [JsonPropertyName("name")] public string Name { get; set; } = default!;
    [JsonPropertyName("isDefault")] public bool IsDefault { get; set; }
}