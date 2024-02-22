using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class UserRepository : DynamoRepository, IUserRepository
{
    private readonly IEventBusManager _eventBusManager;

    public UserRepository(IAmazonDynamoDB dynamoDb, IEventBusManager eventBusManager) : base(dynamoDb)
    {
        _eventBusManager = eventBusManager;
    }

    protected override string GetTableName() => "users";


    public async Task<UserEntity?> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<UserEntity>("users", userId, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await base.DeleteAsync("users", userId, cancellationToken);
        await _eventBusManager.UserDeletedAsync(userId, cancellationToken);
        return response;
    }

    public async Task<bool> SaveAsync(UserEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await base.SaveAsync(entity, cancellationToken);
        await _eventBusManager.UserModifiedAsync(entity, cancellationToken);
        return response;
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

    public async Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await GetAllAsync<UserEntity>("users", cancellationToken);
    }
}