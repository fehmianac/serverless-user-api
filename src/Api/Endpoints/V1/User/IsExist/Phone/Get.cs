using Api.Infrastructure.Contract;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.IsExist.Phone;

public class Get : IEndpoint
{
    private static async Task<IResult> Handler([FromQuery] string phone, [FromServices] IUniqueKeyRepository uniqueKeyRepository, CancellationToken cancellationToken)
    {
        var isExist = await uniqueKeyRepository.GetAsync(phone, UniqueKeyType.Phone, cancellationToken) != null;
        return Results.Ok(isExist);
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/v1/users/phone/check", Handler)
            .Produces<bool>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}