using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class UserRepository : DynamoRepository, IUserRepository
{
    public UserRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => "users";


    public async Task<UserEntity?> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<UserEntity>("users", userId, cancellationToken);
    }

    public async Task<bool> SaveAsync(UserEntity entity, CancellationToken cancellationToken = default)
    {
        return await base.SaveAsync(entity, cancellationToken);
    }

    public async Task<(IList<UserEntity> users, string nextToken)> GetPagedAsync(int limit, string? nextToken, CancellationToken cancellationToken)
    {
        var (users, token, _) = await GetPagedAsync<UserEntity>($"users", nextToken, limit, cancellationToken);
        return (users, token);
    }

    public async Task<IList<UserEntity>> GetUsersAsync(IList<string> userIds, CancellationToken cancellationToken)
    {
        return await BatchGetAsync(userIds.Select(q => new UserEntity
        {
            Id = q
        }).ToList(), cancellationToken);
    }
}