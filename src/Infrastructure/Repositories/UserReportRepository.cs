using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class UserReportRepository : DynamoRepository, IUserReportRepository
{
    public UserReportRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => "users";

    public async Task<bool> SaveAsync(string reporter, string reported, string? reason, CancellationToken cancellationToken = default)
    {
        var entity = new UserReportEntity
        {
            CreatedAt = DateTime.UtcNow,
            Reason = reason,
            ReportedId = reported,
            ReporterId = reporter
        };
        return await base.SaveAsync(entity, cancellationToken);
    }
}