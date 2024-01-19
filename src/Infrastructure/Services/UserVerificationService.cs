using Domain.Entities;
using Domain.Enums;
using Domain.Options;
using Domain.Providers;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class UserVerificationService : IUserVerificationService
{
    private readonly ISmsProvider _smsProvider;
    private readonly IMailProvider _mailProvider;
    private readonly IOtpCodeRepository _otpCodeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEventBusManager _eventBusManager;
    private readonly IOptionsSnapshot<UserVerificationSettings> _userVerificationSettingsOptions;

    public UserVerificationService(
        ISmsProvider smsProvider,
        IMailProvider mailProvider,
        IOtpCodeRepository otpCodeRepository,
        IUserRepository userRepository,
        IEventBusManager eventBusManager,
        IOptionsSnapshot<UserVerificationSettings> userVerificationSettingsOptions)
    {
        _smsProvider = smsProvider;
        _mailProvider = mailProvider;
        _otpCodeRepository = otpCodeRepository;
        _userRepository = userRepository;
        _userVerificationSettingsOptions = userVerificationSettingsOptions;
        _eventBusManager = eventBusManager;
    }

    public async Task<bool> SendVerificationCodeAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        if (_userVerificationSettingsOptions.Value.EmailShouldVerifyOnRegister)
        {
            await SendMailOtpCodeAsync(user, cancellationToken);
        }

        if (_userVerificationSettingsOptions.Value.PhoneShouldVerifyOnRegister)
        {
            await SendSmsOtpCodeAsync(user, cancellationToken);
        }

        return true;
    }

    public async Task<bool> SendVerificationCodeAsync(string userId, UniqueKeyType keyType, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        switch (keyType)
        {
            case UniqueKeyType.Email:
                await SendMailOtpCodeAsync(user, cancellationToken);
                return true;
            case UniqueKeyType.Phone:
                await SendSmsOtpCodeAsync(user, cancellationToken);
                return true;
            case UniqueKeyType.UserName:
            default:

                return false;
        }
    }

    public async Task<bool> SendKeyChangeVerificationCode(string userId, string newKey, UniqueKeyType keyType, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        switch (keyType)
        {
            case UniqueKeyType.EmailUpdateRequest:
                await SendEmailUpdateOtp(user, newKey, cancellationToken);
                return true;
            case UniqueKeyType.PhoneUpdateRequest:
                await SendPhoneUpdateOtp(user, newKey, cancellationToken);
                return true;
            default:

                return false;
        }
    }

    private async Task SendEmailUpdateOtp(UserEntity user, string newKey, CancellationToken cancellationToken)
    {
        var emilCode = await SaveOtpCodeAsync(user.Id, UniqueKeyType.EmailUpdateRequest, cancellationToken);
        if (user.Email != null)
        {
            await _eventBusManager.EmailUpdateOtpRequestedAsync(user.Id, newKey, emilCode, cancellationToken);
        }
    }

    private async Task SendPhoneUpdateOtp(UserEntity user, string newKey, CancellationToken cancellationToken)
    {
        var emilCode = await SaveOtpCodeAsync(user.Id, UniqueKeyType.PhoneUpdateRequest, cancellationToken);
        if (user.Email != null)
        {
            await _eventBusManager.PhoneUpdateOtpRequestedAsync(user.Id, newKey, emilCode, cancellationToken);
        }
    }

    private async Task SendMailOtpCodeAsync(UserEntity user, CancellationToken cancellationToken)
    {
        var emilCode = await SaveOtpCodeAsync(user.Id, UniqueKeyType.Email, cancellationToken);
        if (user.Email != null)
        {
            await _mailProvider.SendMailAsync(user.Email, "Subject", $"VerificationCode: {emilCode}", cancellationToken);
            await _eventBusManager.EmailValidationOtpRequestAsync(user.Id, emilCode, cancellationToken);
        }
    }

    private async Task SendSmsOtpCodeAsync(UserEntity user, CancellationToken cancellationToken)
    {
        var smsCode = await SaveOtpCodeAsync(user.Id, UniqueKeyType.Phone, cancellationToken);
        if (user.Phone != null)
        {
            await _smsProvider.SendSms(user.Phone, $"VerificationCode: {smsCode}", cancellationToken);
            await _eventBusManager.PhoneValidationOtpRequestedAsync(user.Id, smsCode, cancellationToken);
        }
    }

    private async Task<string> SaveOtpCodeAsync(string userId, UniqueKeyType type, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var rnd = new Random();
        var code = rnd.Next(11111, 99999);
        await _otpCodeRepository.SaveOtpCodeAsync(new OtpCodeEntity
        {
            Code = code.ToString(),
            Type = type,
            CreatedAt = utcNow,
            ExpireAt = utcNow.AddMinutes(_userVerificationSettingsOptions.Value.ExpireInXMinute),
            UserId = userId
        }, cancellationToken);
        return code.ToString();
    }
}