namespace Domain.Options;

public class EmailSettings
{
    public bool IsEnabled { get; set; }
    public string? Provider { get; set; }
    public string? From { get; set; }
    public string? FromName { get; set; }
}