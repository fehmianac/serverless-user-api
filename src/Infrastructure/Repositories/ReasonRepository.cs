using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class ReasonRepository : DynamoRepository, IReasonRepository
{
    public ReasonRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => "users";

    public async Task<List<ReasonLookupEntity>> GetReasonLookupAsync(ReasonType type, CancellationToken cancellationToken)
    {
        return await GetAllAsync<ReasonLookupEntity>(ReasonLookupEntity.GetPk(type), cancellationToken);
    }

    public async Task<bool> SaveReasonLookupAsync(ReasonLookupEntity reason, CancellationToken cancellationToken)
    {
        return await SaveAsync(reason, cancellationToken);
    }

    public async Task<bool> SaveReasonAsync(ReasonEntity reason, CancellationToken cancellationToken)
    {
        return await SaveAsync(reason, cancellationToken);
    }
}