using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Suspend;

public class Put : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromBody] UserSuspendRequest request,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserRepository userRepository,
        [FromServices] IReasonRepository reasonRepository,
        [FromServices] IEventBusManager eventBusManager,
        CancellationToken cancellationToken)
    {
        var id = apiContext.CurrentUserId;
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
        
        await eventBusManager.UserHasBeenSuspendedAsync(user.Id, request.ReasonId, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("v1/user/me/suspend", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }

    public record UserSuspendRequest(string ReasonId);
}