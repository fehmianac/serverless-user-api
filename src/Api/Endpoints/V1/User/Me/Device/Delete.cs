using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Me.Device;

public class Delete : IEndpoint
{
    private static async Task<IResult> Handler([FromRoute] string deviceId,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserDeviceRepository userDeviceRepository,
        CancellationToken cancellationToken)
    {
        var userDevice = await userDeviceRepository.GetUserDeviceAsync(apiContext.CurrentUserId, deviceId, cancellationToken);
        if (userDevice == null)
        {
            return Results.NotFound();
        }

        await userDeviceRepository.DeleteUserDeviceAsync(apiContext.CurrentUserId, deviceId, cancellationToken);

        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/v1/users/me/device/{deviceId}", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}