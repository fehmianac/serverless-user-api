using Api.Infrastructure.Contract;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Api.Endpoints.V1.User.Avatar;

public class Put : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string id,
        [FromBody] AvatarPutRequest request,
        [FromServices] IUserRepository userRepository,
        [FromServices] IUniqueKeyRepository uniqueKeyRepository,
        [FromServices] IUserIdentityVerificationService identityVerificationService,
        [FromServices] IStringLocalizer localizer,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(id, cancellationToken);
        if (user == null)
        {
            return Results.NotFound(localizer["UserNotFound"]);
        }

        user.AvatarUrl = request.AvatarUrl;
        if (user.IsVerified && !string.IsNullOrEmpty(user.SelfieUrl))
        {
            var verificationResult =
                await identityVerificationService.VerifyByAvatarAsync(user, user.SelfieUrl, cancellationToken);
            user.IsVerified = verificationResult;
        }

        await userRepository.SaveAsync(user, cancellationToken);


        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/users/{id}/avatar", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}

public class AvatarPutRequest
{
    public string AvatarUrl { get; set; } = default!;
}