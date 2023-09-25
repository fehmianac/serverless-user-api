using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Verification;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromBody] UserIdentityVerifyRequest request,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserIdentityVerificationService userIdentityVerificationService,
        CancellationToken cancellationToken)
    {
        var (identityVerified, _) = await userIdentityVerificationService.CheckIsValidIdentityAsync(request.IdCardUrl, cancellationToken);
        if (!identityVerified)
            return Results.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Detail = "Invalid Id Card"
            });

        var isVerified = await userIdentityVerificationService.CompareFaceAndIdCardAsync(apiContext.CurrentUserId, request.FaceUrl, request.IdCardUrl, cancellationToken);
        return Results.Ok(new UserIdentityVerifyResponse
        {
            IsVerified = isVerified
        });
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("user/v1/verification", Handler)
            .Produces<UserIdentityVerifyResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }

    public class UserIdentityVerifyRequest
    {
        public string IdCardUrl { get; set; } = default!;
        public string FaceUrl { get; set; } = default!;
    }

    public class UserIdentityVerifyResponse
    {
        public bool IsVerified { get; set; }
    }
}