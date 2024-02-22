using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> SaveAsync(UserEntity entity, CancellationToken cancellationToken = default);
    Task<(IList<UserEntity> users, string nextToken)> GetPagedAsync(int limit, string? nextToken, CancellationToken cancellationToken);

    Task<IList<UserEntity>> GetUsersAsync(IList<string> userIds, CancellationToken cancellationToken);
    Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken);
}