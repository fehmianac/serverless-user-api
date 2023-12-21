using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Enums;
using Domain.Options;
using Domain.Repositories;
using Infrastructure.Repositories.Base;
using Microsoft.Extensions.Options;

namespace Infrastructure.Repositories;

public class OtpCodeRepository : DynamoRepository, IOtpCodeRepository
{
    private readonly IOptionsSnapshot<OtpSettings> _otpSettingsOptions;

    public OtpCodeRepository(IAmazonDynamoDB dynamoDb, IOptionsSnapshot<OtpSettings> otpSettingsOptions) : base(dynamoDb)
    {
        _otpSettingsOptions = otpSettingsOptions;
    }

    protected override string GetTableName() => "users";


    public async Task<bool> CheckOtpCodeAsync(string otpCode, string userId, UniqueKeyType keyType, CancellationToken cancellationToken = default)
    {
        var code = await GetAsync<OtpCodeEntity>($"OtpCodes#{keyType}", otpCode, cancellationToken);
        if (_otpSettingsOptions.Value.IsTestMode)
        {
            return (code != null && code.UserId == userId && code.ExpireAt >= DateTime.UtcNow) || (otpCode == _otpSettingsOptions.Value.TestCode);
        }

        return code != null && code.UserId == userId && code.ExpireAt >= DateTime.UtcNow;
    }

    public async Task<bool> SaveOtpCodeAsync(OtpCodeEntity entity, CancellationToken cancellationToken = default)
    {
        return await SaveAsync(entity, cancellationToken);
    }
}