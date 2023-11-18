namespace Domain.Repositories;

public interface IUserReportRepository
{
    Task<bool> SaveAsync(string reporter, string reported, string? reason, CancellationToken cancellationToken = default);
}