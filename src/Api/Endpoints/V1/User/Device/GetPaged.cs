using Api.Infrastructure.Contract;
using Domain.Dto;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Device;

public class GetPaged : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string id,
        [FromQuery] int limit,
        [FromQuery] string? nextToken,
        [FromServices] IUserDeviceRepository userDeviceRepository,
        CancellationToken cancellationToken)
    {
        var (userDevices, token) = await userDeviceRepository.GetUserDevicesPagedAsync(id, limit, nextToken, cancellationToken);


        return Results.Ok(new PagedResponse<UserDeviceDto>
        {
            Data = userDevices.Select(q => q.ToDto()).ToList(),
            Limit = limit,
            NextToken = token,
            PreviousToken = nextToken
        });
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("v1/users/{id}/devices", Handler)
            .Produces<PagedResponse<UserDeviceDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}