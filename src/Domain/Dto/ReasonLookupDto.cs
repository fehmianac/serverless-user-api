using Domain.Entities;

namespace Domain.Dto;

public class ReasonLookupDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
}

public static class ReasonLookupDtoMapper
{
    public static ReasonLookupDto ToDto(this ReasonLookupEntity entity, string culture)
    {
        var translation = entity.Translations.FirstOrDefault(q => q.Culture == culture);

        if (translation == null)
            translation = entity.Translations.FirstOrDefault();

        return new ReasonLookupDto
        {
            Id = entity.Id,
            Name = translation?.Name ?? "----",
        };
    }
}