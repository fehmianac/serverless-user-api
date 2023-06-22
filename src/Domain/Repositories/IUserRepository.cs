using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetAsync(string userId, CancellationToken cancellationToken = default);

    Task<bool> SaveAsync(UserEntity entity, CancellationToken cancellationToken = default);
    Task<(IList<UserEntity> users, string nextToken)> GetPagedAsync(string userId, int limit, string? nextToken, CancellationToken cancellationToken);
}