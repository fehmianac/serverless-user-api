using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories;

public interface IOtpCodeRepository
{
    Task<bool> CheckOtpCodeAsync(string otpCode, string userId, UniqueKeyType keyType, CancellationToken cancellationToken = default);

    Task<bool> SaveOtpCodeAsync(OtpCodeEntity entity, CancellationToken cancellationToken = default);
}