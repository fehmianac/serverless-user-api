using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class UniqueKeyRepository : DynamoRepository, IUniqueKeyRepository
{
    public UniqueKeyRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => "users";

    public async Task<UniqueKeyEntity?> GetAsync(string key, UniqueKeyType keyType, CancellationToken cancellationToken = default)
    {
        return await GetAsync<UniqueKeyEntity>($"uniqueData#{keyType}", key, cancellationToken);
    }

    public async Task<bool> SaveAsync(UniqueKeyEntity entity, CancellationToken cancellationToken = default)
    {
        return await base.SaveAsync(entity, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string key, UniqueKeyType keyType, CancellationToken cancellationToken = default)
    {
        return await base.DeleteAsync($"uniqueData#{keyType}", key, cancellationToken);
    }
}