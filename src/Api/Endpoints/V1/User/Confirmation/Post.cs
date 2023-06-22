using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Enums;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Confirmation;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromBody] VerifyCodePostRequest request,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserRepository userRepository,
        [FromServices] IUserVerificationService userVerificationService,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(apiContext.CurrentUserId, cancellationToken);
        if (user == null)
        {
            return Results.NotFound();
        }

        await userVerificationService.SendVerificationCodeAsync(user.Id, request.KeyType, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/users/confirmation", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}

public class VerifyCodePostRequest
{
    public UniqueKeyType KeyType { get; set; }
}