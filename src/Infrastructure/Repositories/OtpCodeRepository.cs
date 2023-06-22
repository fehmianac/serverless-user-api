using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class OtpCodeRepository : DynamoRepository, IOtpCodeRepository
{
    public OtpCodeRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => "users";


    public async Task<bool> CheckOtpCodeAsync(string otpCode, string userId, UniqueKeyType keyType, CancellationToken cancellationToken = default)
    {
        var code = await GetAsync<OtpCodeEntity>($"OtpCodes#{keyType}", otpCode, cancellationToken);
        return code != null && code.UserId == userId && code.ExpireAt >= DateTime.UtcNow;
    }

    public async Task<bool> SaveOtpCodeAsync(OtpCodeEntity entity, CancellationToken cancellationToken = default)
    {
        return await SaveAsync(entity, cancellationToken);
    }
}