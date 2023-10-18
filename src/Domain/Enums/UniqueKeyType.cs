namespace Domain.Enums;

[System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum UniqueKeyType
{
    UserName = 0,
    Email = 1,
    Phone = 2,
    EmailUpdateRequest = 3,
    PhoneUpdateRequest = 4
}
