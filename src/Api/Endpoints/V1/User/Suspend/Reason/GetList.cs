using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Suspend.Reason;

public class GetList : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromQuery] ReasonType type,
        [FromServices] IApiContext apiContext,
        [FromServices] IReasonRepository reasonRepository,
        CancellationToken cancellationToken
    )
    {
        var reasons = await reasonRepository.GetReasonLookupAsync(type, cancellationToken);

        return Results.Ok(reasons.Select(q => q.ToDto(apiContext.Culture)));
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("v1/user/suspend/reason", Handler)
            .Produces<List<ReasonLookupDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}