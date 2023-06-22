using Domain.Providers;

namespace Infrastructure.Providers;

public class DummySmsProvider : ISmsProvider
{
    public Task<bool> SendSms(string to, string message, CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }
}