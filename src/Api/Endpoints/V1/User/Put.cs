using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Enums;
using Domain.Options;
using Domain.Repositories;
using Domain.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Endpoints.V1.User;

public class Put : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string id,
        [FromBody] UserPutRequest request,
        [FromServices] IUserRepository userRepository,
        [FromServices] IUniqueKeyRepository uniqueKeyRepository,
        [FromServices] IOptionsSnapshot<UniqueKeySettings> uniqueKeySettingsOptions,
        [FromServices] IUserVerificationService userVerificationService,
        [FromServices] IUserIdentityVerificationService identityVerificationService,
        [FromServices] IValidator<UserPutRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var utcNow = DateTime.UtcNow;
        var user = await userRepository.GetAsync(id, cancellationToken);
        var oldUser = user;

        if (oldUser != null)
            oldUser.Email = oldUser.Email?.ToLower();

        var isRegisterState = user == null;
        request.Email = request.Email?.ToLower();

        var isVerified = false;
        if (user is { IsVerified: true }
            && user.AvatarUrl != request.AvatarUrl
            && !string.IsNullOrEmpty(request.AvatarUrl)
            && !string.IsNullOrEmpty(user.SelfieUrl))
        {
            var verificationResult =
                await identityVerificationService.VerifyByAvatarAsync(user, user.SelfieUrl, cancellationToken);
            isVerified = verificationResult;
        }

        user = new UserEntity
        {
            Email = request.Email,
            Gender = request.Gender,
            BirthDate = request.BirthDate,
            AdditionalData = request.AdditionalData,
            SettingsData = request.SettingsData,
            DefaultLanguage = request.DefaultLanguage,
            FirstName = request.FirstName,
            AvatarUrl = request.AvatarUrl,
            LastName = request.LastName,
            IsVerified = isVerified,
            UserName = request.UserName,
            Phone = request.Phone,
            Status = request.Status,
            Id = id,
            CreatedAt = utcNow,
            UpdatedAt = utcNow,
            EmailIsValid = false,
            PhoneIsValid = false
        };

        user.UpdatedAt = utcNow;

        if (uniqueKeySettingsOptions.Value.PhoneShouldBeUnique)
        {
            var check = await CheckUniqueKey(uniqueKeyRepository, user.Id, UniqueKeyType.Phone, user.Phone,
                cancellationToken);
            if (!check)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    {
                        "Phone", new[] { "Phone already exists" }
                    }
                });
            }
        }

        if (uniqueKeySettingsOptions.Value.EmailShouldBeUnique)
        {
            var check = await CheckUniqueKey(uniqueKeyRepository, user.Id, UniqueKeyType.Email, user.Email,
                cancellationToken);
            if (!check)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    {
                        "Email", new[] { "Email already exists" }
                    }
                });
            }
        }

        if (uniqueKeySettingsOptions.Value.UserNameShouldBeUnique)
        {
            var check = await CheckUniqueKey(uniqueKeyRepository, user.Id, UniqueKeyType.UserName, user.UserName,
                cancellationToken);
            if (!check)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    {
                        "UserName", new[] { "UserName already exists" }
                    }
                });
            }
        }

        await userRepository.SaveAsync(user, cancellationToken);


        if (uniqueKeySettingsOptions.Value.PhoneShouldBeUnique)
        {
            await OrganizeUniqueKey(uniqueKeyRepository, UniqueKeyType.Phone, user.Id, oldUser?.Phone, user.Phone,
                cancellationToken);
        }

        if (uniqueKeySettingsOptions.Value.EmailShouldBeUnique)
        {
            await OrganizeUniqueKey(uniqueKeyRepository, UniqueKeyType.Email, user.Id, oldUser?.Email, user.Email,
                cancellationToken);
        }

        if (uniqueKeySettingsOptions.Value.UserNameShouldBeUnique)
        {
            await OrganizeUniqueKey(uniqueKeyRepository, UniqueKeyType.UserName, user.Id, oldUser?.UserName,
                user.UserName, cancellationToken);
        }

        if (!isRegisterState)
        {
            return Results.Ok();
        }

        await userVerificationService.SendVerificationCodeAsync(user.Id, cancellationToken);

        return Results.Ok();
    }


    private static async Task<bool> CheckUniqueKey(IUniqueKeyRepository uniqueKeyRepository, string userId,
        UniqueKeyType keyType, string? key, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(key))
            return true;

        var check = await uniqueKeyRepository.GetAsync(key, keyType, cancellationToken);
        return check == null || check.UserId == userId;
    }

    private static async Task OrganizeUniqueKey(IUniqueKeyRepository uniqueKeyRepository, UniqueKeyType keyType,
        string userId, string? oldKey, string? key, CancellationToken cancellationToken)
    {
        if (oldKey == key)
        {
            return;
        }

        if (!string.IsNullOrEmpty(oldKey))
        {
            await uniqueKeyRepository.DeleteAsync(oldKey, keyType, cancellationToken);
        }

        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        await uniqueKeyRepository.SaveAsync(new UniqueKeyEntity
        {
            Value = key,
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            Type = keyType
        }, cancellationToken);
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/users/{id}", Handler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("User");
    }
}

public class UserPutRequest
{
    public string? UserName { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Status { get; set; } = default!;
    public bool IsVerified { get; set; }
    public string? DefaultLanguage { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = new();
    public Dictionary<string, string> SettingsData { get; set; } = new();

    public class UserPutRequestValidator : AbstractValidator<UserPutRequest>
    {
        public UserPutRequestValidator()
        {
            RuleFor(q => q.FirstName).NotEmpty();
        }
    }
}