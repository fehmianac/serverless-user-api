using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class VerifyLogRepository : DynamoRepository, IVerifyLogRepository
{
    public VerifyLogRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => "users";


    public async Task<bool> SaveAsync(VerifyLogEntity entity, CancellationToken cancellationToken = default)
    {
        return await base.SaveAsync(entity, cancellationToken);
    }

    public async Task<bool> DeleteUserVerifyLogsAsync(string userId, CancellationToken cancellationToken)
    {
        var entities = await GetAllAsync<VerifyLogEntity>($"verifyLogs#{userId}#", cancellationToken);
        if (entities.Count == 0)
            return true;
        await BatchWriteAsync(new List<IEntity>(), new List<IEntity>(entities), cancellationToken);
        return true;
    }
}