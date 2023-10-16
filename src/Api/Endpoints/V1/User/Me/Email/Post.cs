using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Me.Email;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromBody] UserUpdateEmailRequest request,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserRepository userRepository,
        [FromServices] IOtpCodeRepository otpCodeRepository,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(apiContext.CurrentUserId, cancellationToken);
        if (user == null)
        {
            return Results.NotFound();
        }

        var otpCodeIsValid = await otpCodeRepository.CheckOtpCodeAsync(apiContext.CurrentUserId, request.Code, UniqueKeyType.EmailUpdateRequest, cancellationToken);
        if (!otpCodeIsValid)
        {
            return Results.NotFound();
        }

        user.Email = request.Email;
        await userRepository.SaveAsync(user, cancellationToken);

        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/users/me/email", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }

    public class UserUpdateEmailRequest
    {
        public string Code { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}