using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Me.Device;

public class Put : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string deviceId,
        [FromServices] IApiContext apiContext,
        [FromBody] UserMeDevicePutRequest request,
        [FromServices] IUserDeviceRepository userDeviceRepository,
        CancellationToken cancellationToken)
    {
        var userDevice = await userDeviceRepository.GetUserDeviceAsync(apiContext.CurrentUserId, deviceId, cancellationToken) ?? new UserDeviceEntity
        {
            Id = deviceId,
            UserId = apiContext.CurrentUserId
        };

        userDevice.Platform = request.Platform;
        userDevice.AdditionalData = request.AdditionalData;
        await userDeviceRepository.SaveUserDeviceAsync(userDevice, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/v1/users/me/device/{deviceId}", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}

public class UserMeDevicePutRequest
{
    public string? Platform { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = new();
}