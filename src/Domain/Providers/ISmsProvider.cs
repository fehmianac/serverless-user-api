namespace Domain.Providers;

public interface ISmsProvider
{
    Task<bool> SendSms(string to, string message, CancellationToken cancellationToken);
}