using Api.Infrastructure.Contract;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.IsExist.Email.Id;

public class Get : IEndpoint
{
    private static async Task<IResult> Handler([FromQuery] string email,
        [FromServices] IUniqueKeyRepository uniqueKeyRepository, CancellationToken cancellationToken)
    {
        email = email.ToLower();
        var value = await uniqueKeyRepository.GetAsync(email, UniqueKeyType.Email, cancellationToken);

        if (value == null)
            return Results.NotFound();

        return Results.Ok(value.UserId);
    }
    
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/v1/users/email/id", Handler)
            .Produces<string>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}