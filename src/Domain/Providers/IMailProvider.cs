namespace Domain.Providers;

public interface IMailProvider
{
    public Task<bool> SendMailAsync(string to, string subject, string body, CancellationToken cancellationToken);
}