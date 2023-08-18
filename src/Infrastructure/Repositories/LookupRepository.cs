using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class LookupRepository : DynamoRepository, ILookupRepository
{
    public LookupRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    public async Task<bool> SaveAsync(LookupDefinitionEntity entity, CancellationToken cancellationToken = default)
    {
        return await base.SaveAsync(entity, cancellationToken);
    }

    public async Task<LookupDefinitionEntity?> GetAsync(string type, string id, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<LookupDefinitionEntity>(LookupDefinitionEntity.GetPk(), type + "#" + id, cancellationToken);
    }

    public async Task<List<LookupDefinitionEntity>> GetListAsync(string type, CancellationToken cancellationToken = default)
    {
        return await base.GetAllAsync<LookupDefinitionEntity>(LookupDefinitionEntity.GetPk(), type, SkOperator.BeginsWith, cancellationToken);
    }

    public async Task<List<LookupDefinitionEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await base.GetAllAsync<LookupDefinitionEntity>(LookupDefinitionEntity.GetPk(), cancellationToken);
    }


    protected override string GetTableName() => "users";
}