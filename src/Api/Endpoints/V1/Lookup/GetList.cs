using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Endpoints.V1.Lookup;

public class GetList : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromQuery] string? type,
        [FromServices] IApiContext apiContext,
        [FromServices] ILookupRepository lookupRepository,
        [FromServices] IMemoryCache memoryCache,
        [FromServices] IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        var allUsers = await userRepository.GetAllAsync(cancellationToken);
        var user = allUsers.Where(q => q.CreatedAt > new DateTime(2024,6,19));
        var phones = user.Select(q => q.Phone);
        var emails = user.Select(q => q.Email);

        var emailsWithComma = string.Join(',', emails);

        var phoneWithComma = string.Join(',', phones);
        
        var cacheKey = $"lookup-{type}-{apiContext.Culture}";
        var cacheResult = memoryCache.Get<List<LookupDefinitionDto>>(cacheKey);
        if (cacheResult != null)
        {
            return Results.Ok(cacheResult);
        }

        List<LookupDefinitionEntity> lookups;
        if (!string.IsNullOrEmpty(type))
        {
            lookups = await lookupRepository.GetListAsync(type, cancellationToken);
        }
        else
        {
            lookups = await lookupRepository.GetAllAsync(cancellationToken);
        }

        var result = lookups.Select(x => x.ToDto(apiContext.Culture)).OrderBy(q => q.Rank).ToList();
        memoryCache.Set(cacheKey, result, TimeSpan.FromHours(1));
        return Results.Ok(result);
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("v1/lookup", Handler).Produces<List<LookupDefinitionDto>>().WithTags("Lookup");
    }
}