using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Device;

public class Put : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string id,
        [FromRoute] string deviceId,
        [FromBody] UserDevicePutRequest request,
        [FromServices] IUserDeviceRepository userDeviceRepository,
        CancellationToken cancellationToken)
    {
        var userDevice = await userDeviceRepository.GetUserDeviceAsync(id, deviceId, cancellationToken) ?? new UserDeviceEntity
        {
            Id = deviceId,
            UserId = id
        };

        userDevice.Platform = request.Platform;
        userDevice.AdditionalData = request.AdditionalData;
        await userDeviceRepository.SaveUserDeviceAsync(userDevice, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/v1/users/{id}/device/{deviceId}", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}

public class UserDevicePutRequest
{
    public string? Platform { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = new();
}