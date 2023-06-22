using Domain.Providers;

namespace Infrastructure.Providers;

public class DummyEmailProvider : IMailProvider
{
    public Task<bool> SendMailAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}