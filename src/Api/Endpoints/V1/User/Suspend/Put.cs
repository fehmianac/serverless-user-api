using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Suspend;

public class Put : IEndpoint
{
    private static async Task<IResult> Handler([FromRoute] string id,
        [FromBody] UserSuspendRequest request,
        [FromServices] IUserRepository userRepository,
        [FromServices] IReasonRepository reasonRepository,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(id, cancellationToken);
        if (user == null)
            return Results.NotFound();

        user.Status = "suspended";
        await userRepository.SaveAsync(user, cancellationToken);
        await reasonRepository.SaveReasonAsync(new ReasonEntity
        {
            ReasonId = request.ReasonId,
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id,
            Type = ReasonType.Suspend
        }, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("v1/user/{id}/suspend", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }

    public record UserSuspendRequest(string ReasonId);
}