using System.Collections.ObjectModel;
using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Verification.IdCard;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler([FromBody] CheckIdCardRequest request,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserIdentityVerificationService userIdentityVerificationService,
        CancellationToken cancellationToken)
    {
        var (identityVerified, labels) = await userIdentityVerificationService.CheckIsValidIdentityAsync(request.IdCardUrl, cancellationToken);
        return Results.Ok(new CheckIdCardResponse
        {
            IsValidIdCard = identityVerified,
            Labels = new ReadOnlyCollection<string>(labels)
        });
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/verification/id-card/check", Handler)
            .Produces<CheckIdCardResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }

    public class CheckIdCardRequest
    {
        public string IdCardUrl { get; set; } = default!;
    }

    public class CheckIdCardResponse
    {
        public bool IsValidIdCard { get; set; }
        public ReadOnlyCollection<string> Labels { get; set; } = default!;
    }
}