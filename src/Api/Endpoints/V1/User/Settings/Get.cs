using Api.Infrastructure.Contract;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Settings;

public class Get : IEndpoint
{
    private static async Task<IResult> Handler([FromRoute] string id,
        [FromServices] IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(id, cancellationToken);
        return user == null ? Results.NotFound() : Results.Ok(user.SettingsData);
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/v1/users/{id}/settings", Handler)
            .Produces<Dictionary<string, string>>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}