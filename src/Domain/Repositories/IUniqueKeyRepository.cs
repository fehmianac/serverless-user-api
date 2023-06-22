using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories;

public interface IUniqueKeyRepository
{
    Task<UniqueKeyEntity?> GetAsync(string key, UniqueKeyType keyType, CancellationToken cancellationToken = default);

    Task<bool> SaveAsync(UniqueKeyEntity entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string key, UniqueKeyType keyType, CancellationToken cancellationToken = default);
}