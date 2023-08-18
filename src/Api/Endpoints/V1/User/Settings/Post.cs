using Api.Infrastructure.Contract;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Settings;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string id,
        [FromBody] Dictionary<string, string> request,
        [FromServices] IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(id, cancellationToken);
        if (user == null)
            return Results.NotFound();
        user.SettingsData = request;
        await userRepository.SaveAsync(user, cancellationToken);
        return Results.NoContent();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/users/{id}/settings", Handler)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}