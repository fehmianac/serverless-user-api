using Domain.Entities;

namespace Domain.Repositories;

public interface IVerifyLogRepository
{
    Task<bool> SaveAsync(VerifyLogEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserVerifyLogsAsync(string userId, CancellationToken cancellationToken);
}