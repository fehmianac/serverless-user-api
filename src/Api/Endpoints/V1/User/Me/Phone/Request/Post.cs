using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Enums;
using Domain.Repositories;
using Domain.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Me.Phone.Request;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromBody] UserChangePhoneRequest request,
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

        await userVerificationService.SendKeyChangeVerificationCode(user.Id, request.Phone, UniqueKeyType.PhoneUpdateRequest, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/users/me/phone/change/request", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }

    public record UserChangePhoneRequest(string Phone);

    public class UserChangePhoneRequestValidator : AbstractValidator<UserChangePhoneRequest>
    {
        public UserChangePhoneRequestValidator()
        {
            RuleFor(q => q.Phone).NotEmpty();
        }
    }
}