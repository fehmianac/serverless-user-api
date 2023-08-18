using Api.Infrastructure.Contract;
using Domain.Dto;
using Domain.Entities;
using Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Lookup;

public class Put : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string id,
        [FromBody] TranslationPutRequest request,
        [FromServices] ILookupRepository lookupRepository,
        [FromServices] IValidator<TranslationPutRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var entity = new LookupDefinitionEntity
        {
            Translations = request.Translations,
            Name = request.Name,
            Type = request.Type,
            Id = id
        };

        await lookupRepository.SaveAsync(entity, cancellationToken);
        return Results.NoContent();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("v1/lookup/{id}", Handler).Produces(StatusCodes.Status204NoContent).WithTags("Lookup");
    }

    public record TranslationPutRequest(string Type, string Name, List<TranslationDto> Translations);

    public class TranslationPutRequestValidator : AbstractValidator<TranslationPutRequest>
    {
        public TranslationPutRequestValidator()
        {
            RuleFor(q => q.Type).NotEmpty();
            RuleFor(q => q.Name).NotEmpty();
            RuleFor(q => q.Translations).NotEmpty();
        }
    }
}