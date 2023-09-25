using Api.Infrastructure.Contract;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Suspend;

public class Delete : IEndpoint
{
    private async Task<IResult> Handler([FromRoute] string id,
        [FromBody] UserSuspendDeleteRequest request,
        [FromServices] IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(id, cancellationToken);
        if (user == null)
            return Results.NotFound();

        user.Status = request.Status;
        await userRepository.SaveAsync(user, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("v1/user/{id}/suspend", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }

    public record UserSuspendDeleteRequest(string Status);
}