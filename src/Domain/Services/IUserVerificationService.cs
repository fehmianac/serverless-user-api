using Domain.Enums;

namespace Domain.Services;

public interface IUserVerificationService
{
    Task<bool> SendVerificationCodeAsync(string userId, CancellationToken cancellationToken);
    Task<bool> SendVerificationCodeAsync(string userId, UniqueKeyType keyType, CancellationToken cancellationToken);
    Task<bool> SendKeyChangeVerificationCode(string userId, string requestPhone, UniqueKeyType phoneUpdateRequest, CancellationToken cancellationToken);
}
