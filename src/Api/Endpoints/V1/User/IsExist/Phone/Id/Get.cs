using Api.Infrastructure.Contract;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.IsExist.Phone.Id;

public class Get : IEndpoint
{
    private static async Task<IResult> Handler([FromQuery] string phone, [FromServices] IUniqueKeyRepository uniqueKeyRepository, CancellationToken cancellationToken)
    {
        var value = await uniqueKeyRepository.GetAsync(phone, UniqueKeyType.Phone, cancellationToken);
        if (value == null)
            return Results.NotFound();
        
        return Results.Ok(value.UserId);
    }
    
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/v1/users/phone/id", Handler)
            .Produces<string>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}