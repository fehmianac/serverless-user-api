namespace Domain.Options;

public class UserVerificationSettings
{
    public bool EmailShouldVerifyOnRegister { get; set; }
    public bool PhoneShouldVerifyOnRegister { get; set; }

    public int ExpireInXMinute { get; set; }
}