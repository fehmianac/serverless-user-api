using Api.Infrastructure.Contract;
using Domain.Enums;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User;

public class Delete : IEndpoint
{
    private static async Task<IResult> Handler([FromRoute] string id,
        [FromServices] IUserRepository userRepository,
        [FromServices] IUserDeviceRepository userDeviceRepository,
        [FromServices] IUniqueKeyRepository uniqueKeyRepository,
        [FromServices] IVerifyLogRepository verifyLogRepository,
        [FromServices] IEventBusManager eventBusManager,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(id, cancellationToken);
        if (user == null)
        {
            return Results.NotFound();
        }

        await userRepository.DeleteAsync(id, cancellationToken);
        if (!string.IsNullOrEmpty(user.Phone))
        {
            await uniqueKeyRepository.DeleteAsync(user.Phone, UniqueKeyType.Phone, cancellationToken);
        }

        if (!string.IsNullOrEmpty(user.Email))
        {
            await uniqueKeyRepository.DeleteAsync(user.Email, UniqueKeyType.Email, cancellationToken);
        }

        if (!string.IsNullOrEmpty(user.UserName))
        {
            await uniqueKeyRepository.DeleteAsync(user.UserName, UniqueKeyType.UserName, cancellationToken);
        }

        await userDeviceRepository.DeleteUserDevicesAsync(id, cancellationToken);
        await verifyLogRepository.DeleteUserVerifyLogsAsync(id, cancellationToken);
        await eventBusManager.UserDeletedAsync(id, cancellationToken);
        return Results.NoContent();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/v1/users/{id}", Handler)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}