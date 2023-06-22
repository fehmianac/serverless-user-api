using Api.Infrastructure.Contract;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.IsExist.Email;

public class Get : IEndpoint
{
    private static async Task<IResult> Handler([FromQuery] string email, [FromServices] IUniqueKeyRepository uniqueKeyRepository, CancellationToken cancellationToken)
    {
        var isExist = await uniqueKeyRepository.GetAsync(email, UniqueKeyType.Email, cancellationToken) != null;
        return Results.Ok(isExist);
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/v1/users/email/check", Handler)
            .Produces<bool>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}