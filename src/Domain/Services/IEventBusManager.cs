using Domain.Entities;

namespace Domain.Services;

public interface IEventBusManager
{
    Task<bool> UserModifiedAsync(UserEntity user, CancellationToken cancellationToken = default);

    Task<bool> OtpVerifiedAsync(string userId, string verifiedField, CancellationToken cancellationToken = default);
    Task<bool> DeviceAddedAsync(UserDeviceEntity userDevice, CancellationToken cancellationToken = default);
    Task<bool> DeviceRemovedAsync(UserDeviceEntity userDevice, CancellationToken cancellationToken = default);
    Task<bool> UserDeletedAsync(string userId, CancellationToken cancellationToken);
    Task<bool> IdentityVerifiedAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> EmailValidationOtpRequestAsync(string userId, string emilCode, CancellationToken cancellationToken);
    Task<bool> PhoneValidationOtpRequestedAsync(string userId, string smsCode, CancellationToken cancellationToken);
    Task<bool> EmailUpdateOtpRequestedAsync(string userId, string newKey, string code, CancellationToken cancellationToken);
    Task<bool> PhoneUpdateOtpRequestedAsync(string userId, string newKey, string code, CancellationToken cancellationToken);
    Task<bool> UserHasBeenSuspendedAsync(string userId, string requestReasonId, CancellationToken cancellationToken);
}