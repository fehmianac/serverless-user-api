using Domain.Entities;

namespace Domain.Services;

public interface IEventBusManager
{
    Task<bool> UserModifiedAsync(UserEntity user, CancellationToken cancellationToken = default);

    Task<bool> OtpVerifiedAsync(string userId, string verifiedField, CancellationToken cancellationToken = default);
    Task<bool> DeviceAddedAsync(UserDeviceEntity userDevice, CancellationToken cancellationToken = default);
    Task<bool> DeviceRemovedAsync(UserDeviceEntity userDevice, CancellationToken cancellationToken = default);
    Task<bool> UserDeletedAsync(string userId, CancellationToken cancellationToken);
}