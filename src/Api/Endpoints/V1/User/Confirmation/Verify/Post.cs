using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Confirmation.Verify;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromBody] VerifyPostRequest request,
        [FromServices] IApiContext apiContext,
        [FromServices] IUserRepository userRepository,
        [FromServices] IOtpCodeRepository otpCodeRepository,
        [FromServices] IVerifyLogRepository verifyLogRepository,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(apiContext.CurrentUserId, cancellationToken);
        if (user == null)
        {
            return Results.NotFound();
        }

        var key = request.KeyType switch
        {
            UniqueKeyType.Email => user.Email,
            UniqueKeyType.Phone => user.Phone,
            _ => throw new ArgumentOutOfRangeException()
        };

        var isValid = key != null && await otpCodeRepository.CheckOtpCodeAsync(request.Code, key, request.KeyType, cancellationToken);

        if (!isValid)
        {
            return Results.BadRequest();
        }

        switch (request.KeyType)
        {
            case UniqueKeyType.Email:
                user.EmailIsValid = true;
                break;
            case UniqueKeyType.Phone:
                user.PhoneIsValid = true;
                break;
            case UniqueKeyType.UserName:
            default:
                throw new ArgumentOutOfRangeException();
        }

        await userRepository.SaveAsync(user, cancellationToken);
        await verifyLogRepository.SaveAsync(new VerifyLogEntity
        {
            Type = request.KeyType,
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id
        }, cancellationToken);

        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/users/confirmation/verify", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}

public class VerifyPostRequest
{
    public UniqueKeyType KeyType { get; set; }
    public string Code { get; set; } = default!;
}