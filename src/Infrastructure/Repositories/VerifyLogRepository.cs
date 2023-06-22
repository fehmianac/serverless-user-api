using Amazon.DynamoDBv2;
using Domain.Entities;
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
}