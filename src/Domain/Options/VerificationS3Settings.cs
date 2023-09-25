namespace Domain.Options;

public class VerificationS3Settings
{
    public string BucketName { get; set; } = default!;
    public float MinConfidence { get; set; } = 77L;
}