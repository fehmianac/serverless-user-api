using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Lookup;

public class GetList : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromQuery] string? type,
        [FromServices] IApiContext apiContext,
        [FromServices] ILookupRepository lookupRepository,
        CancellationToken cancellationToken)
    {
        List<LookupDefinitionEntity> lookups;
        if (!string.IsNullOrEmpty(type))
        {
            lookups = await lookupRepository.GetListAsync(type, cancellationToken);
        }
        else
        {
            lookups = await lookupRepository.GetAllAsync(cancellationToken);
        }

        return Results.Ok(lookups.Select(x => x.ToDto(apiContext.Culture)).ToList());
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("v1/lookup", Handler).Produces<List<LookupDefinitionDto>>().WithTags("Lookup");
    }
}