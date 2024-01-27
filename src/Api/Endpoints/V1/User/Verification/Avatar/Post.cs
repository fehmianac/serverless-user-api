using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Verification.Avatar;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromBody] VerifyByAvatarRequest request,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserRepository userRepository,
        [FromServices] IUserIdentityVerificationService identityVerificationService,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(apiContext.CurrentUserId, cancellationToken);
        if (user == null)
            return Results.NotFound();

        var verificationResult =
            await identityVerificationService.VerifyByAvatarAsync(user, request.SelfieUrl, cancellationToken);

        return Results.Ok(verificationResult);
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/users/me/verification/avatar", Handler)
            .Produces<bool>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public record VerifyByAvatarRequest(string SelfieUrl);
}