using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories;

public interface IReasonRepository
{
    Task<List<ReasonLookupEntity>> GetReasonLookupAsync(ReasonType type, CancellationToken cancellationToken);
    
    Task<bool> SaveReasonLookupAsync(ReasonLookupEntity reason, CancellationToken cancellationToken);

    Task<bool> SaveReasonAsync(ReasonEntity reason, CancellationToken cancellationToken);
}