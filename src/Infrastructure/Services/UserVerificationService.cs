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
    private readonly IOptionsSnapshot<UserVerificationSettings> _userVerificationSettingsOptions;

    public UserVerificationService(
        ISmsProvider smsProvider,
        IMailProvider mailProvider,
        IOtpCodeRepository otpCodeRepository,
        IUserRepository userRepository,
        IOptionsSnapshot<UserVerificationSettings> userVerificationSettingsOptions)
    {
        _smsProvider = smsProvider;
        _mailProvider = mailProvider;
        _otpCodeRepository = otpCodeRepository;
        _userRepository = userRepository;
        _userVerificationSettingsOptions = userVerificationSettingsOptions;
    }

    public async Task<bool> SendVerificationCodeAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        var utcNow = DateTime.UtcNow;
        if (_userVerificationSettingsOptions.Value.EmailShouldVerifyOnRegister)
        {
            await SendMailOtpCodeAsync(user, utcNow, cancellationToken);
        }

        if (_userVerificationSettingsOptions.Value.PhoneShouldVerifyOnRegister)
        {
            await SendSmsOtpCodeAsync(user, utcNow, cancellationToken);
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
                await SendMailOtpCodeAsync(user, DateTime.UtcNow, cancellationToken);
                return true;
            case UniqueKeyType.Phone:
                await SendSmsOtpCodeAsync(user, DateTime.UtcNow, cancellationToken);
                return true;
            case UniqueKeyType.UserName:
            default:
                return false;
        }
    }

    private async Task SendMailOtpCodeAsync(UserEntity user, DateTime utcNow, CancellationToken cancellationToken)
    {
        var rnd = new Random();
        var emilCode = rnd.Next(11111, 99999);
        await _otpCodeRepository.SaveOtpCodeAsync(new OtpCodeEntity
        {
            Code = emilCode.ToString(),
            Type = UniqueKeyType.Email,
            CreatedAt = utcNow,
            ExpireAt = utcNow.AddMinutes(_userVerificationSettingsOptions.Value.ExpireInXMinute),
            UserId = user.Id
        }, cancellationToken);
        if (user.Email != null)
            await _mailProvider.SendMailAsync(user.Email, "Subject", $"VerificationCode: {emilCode}", cancellationToken);
    }

    private async Task SendSmsOtpCodeAsync(UserEntity user, DateTime utcNow, CancellationToken cancellationToken)
    {
        var rnd = new Random();
        var smsCode = rnd.Next(11111, 99999);
        await _otpCodeRepository.SaveOtpCodeAsync(new OtpCodeEntity
        {
            Code = smsCode.ToString(),
            Type = UniqueKeyType.Phone,
            CreatedAt = utcNow,
            ExpireAt = utcNow.AddMinutes(_userVerificationSettingsOptions.Value.ExpireInXMinute),
            UserId = user.Id
        }, cancellationToken);

        if (user.Phone != null)
            await _smsProvider.SendSms(user.Phone, $"VerificationCode: {smsCode}", cancellationToken);
    }
}