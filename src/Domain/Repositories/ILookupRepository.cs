using Domain.Entities;

namespace Domain.Repositories;

public interface ILookupRepository
{
    Task<bool> SaveAsync(LookupDefinitionEntity entity, CancellationToken cancellationToken = default);
    Task<LookupDefinitionEntity?> GetAsync(string type, string id, CancellationToken cancellationToken = default);
    Task<List<LookupDefinitionEntity>> GetListAsync(string type, CancellationToken cancellationToken = default);
    Task<List<LookupDefinitionEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}