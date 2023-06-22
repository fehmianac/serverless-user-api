namespace Domain.Options;

public class UniqueKeySettings
{
    public bool EmailShouldBeUnique { get; set; }
    public bool PhoneShouldBeUnique { get; set; }
    public bool UserNameShouldBeUnique { get; set; }
}