using Amazon.DynamoDBv2;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class UserDeviceRepository : DynamoRepository, IUserDeviceRepository
{
    private readonly IEventBusManager _eventBusManager;

    public UserDeviceRepository(IAmazonDynamoDB dynamoDb, IEventBusManager eventBusManager) : base(dynamoDb)
    {
        _eventBusManager = eventBusManager;
    }

    public async Task<UserDeviceEntity?> GetUserDeviceAsync(string userId, string deviceId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<UserDeviceEntity>(UserDeviceEntity.GetPk(userId), deviceId, cancellationToken);
    }

    public async Task<bool> SaveUserDeviceAsync(UserDeviceEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await SaveAsync(entity, cancellationToken);
        await _eventBusManager.DeviceAddedAsync(entity, cancellationToken);
        return response;
    }

    public async Task<bool> DeleteUserDeviceAsync(string userId, string deviceId, CancellationToken cancellationToken = default)
    {
        var device = await GetUserDeviceAsync(userId, deviceId, cancellationToken);
        if (device == null)
            return true;

        var response = await DeleteAsync($"userDevices#{userId}#", deviceId, cancellationToken);
        await _eventBusManager.DeviceRemovedAsync(device, cancellationToken);
        return response;
    }

    public async Task<(List<UserDeviceEntity>, string)> GetUserDevicesPagedAsync(string userId, int limit, string? nextToken, CancellationToken cancellationToken = default)
    {
        var (entities, token, _) = await GetPagedAsync<UserDeviceEntity>($"userDevices#{userId}#", nextToken, limit, cancellationToken);
        return (entities, token);
    }

    public async Task<bool> DeleteUserDevicesAsync(string userId, CancellationToken cancellationToken)
    {
        var entities = await GetAllAsync<UserDeviceEntity>($"userDevices#{userId}#", cancellationToken);
        if (entities.Count == 0)
            return true;
        await BatchWriteAsync(new List<IEntity>(), new List<IEntity>(entities), cancellationToken);
        return true;
    }


    protected override string GetTableName() => "users";
}