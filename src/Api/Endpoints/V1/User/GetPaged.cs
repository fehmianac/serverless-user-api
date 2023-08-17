using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User;

public class GetPaged : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromQuery] string? ids,
        [FromQuery] int limit,
        [FromQuery] string? nextToken,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(ids))
        {
            var foundUsers = await userRepository.GetUsersAsync(ids.Split(','), cancellationToken);
            return Results.Ok(new PagedResponse<UserDto>
            {
                Data = foundUsers.Select(q => q.ToDto()).ToList(),
                Limit = limit,
                NextToken = null,
                PreviousToken = nextToken
            });
        }

        var (users, token) = await userRepository.GetPagedAsync(limit, nextToken, cancellationToken);
        if (!users.Any())
        {
            return Results.Ok(new PagedResponse<UserDto>
            {
                Data = new List<UserDto>(),
                Limit = limit,
                NextToken = null,
                PreviousToken = nextToken
            });
        }

        return Results.Ok(new PagedResponse<UserDto>
        {
            Data = users.Select(q => q.ToDto()).ToList(),
            Limit = limit,
            NextToken = token,
            PreviousToken = nextToken
        });
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("v1/users", Handler)
            .Produces<PagedResponse<UserDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}