using Domain.Entities;

namespace Domain.Dto;

public class LookupDefinitionDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
    public int Rank { get; set; }
}

public static class LookupDefinitionDtoMapper
{
    public static LookupDefinitionDto ToDto(this LookupDefinitionEntity entity, string culture)
    {
        var translation = entity.Translations.FirstOrDefault(q => q.Culture == culture);

        if (translation == null)
            translation = entity.Translations.FirstOrDefault();

        return new LookupDefinitionDto
        {
            Id = entity.Id,
            Name = translation?.Name ?? "----",
            Type = entity.Type,
            Rank = entity.Rank,
        };
    }
}