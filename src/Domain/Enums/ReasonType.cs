namespace Domain.Enums;

[System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum ReasonType
{
    Suspend,
    Delete,
}
