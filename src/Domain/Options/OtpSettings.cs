namespace Domain.Options;

public class OtpSettings
{
    public bool IsTestMode { get; set; }
    public string TestCode { get; set; } = default!;
}