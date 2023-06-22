using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Me;

public class Get : IEndpoint
{
    private static async Task<IResult> Handler([FromServices] IApiContext apiContext, [FromServices] IUserRepository userRepository, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(apiContext.CurrentUserId, cancellationToken);
        return user == null ? Results.NotFound() : Results.Ok(user.ToDto());
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/v1/users/me", Handler)
            .Produces<UserDto>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}