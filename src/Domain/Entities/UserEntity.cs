using System.Text.Json.Serialization;
using Domain.Entities.Base;

namespace Domain.Entities;

public class UserEntity : IEntity
{
    [JsonPropertyName("pk")] public string Pk => "users";
    [JsonPropertyName("sk")] public string Sk => Id;
    [JsonPropertyName("id")] public string Id { get; set; } = default!;
    [JsonPropertyName("userName")] public string? UserName { get; set; }
    [JsonPropertyName("firstName")] public string FirstName { get; set; } = default!;
    [JsonPropertyName("lastName")] public string LastName { get; set; } = default!;
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("phone")] public string? Phone { get; set; }
    [JsonPropertyName("avatarUrl")] public string? AvatarUrl { get; set; }
    [JsonPropertyName("selfieUrl")] public string? SelfieUrl { get; set; }
    [JsonPropertyName("gender")] public string? Gender { get; set; }
    [JsonPropertyName("birthDate")] public DateTime? BirthDate { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = default!;
    [JsonPropertyName("emailIsValid")] public bool EmailIsValid { get; set; }
    [JsonPropertyName("phoneIsValid")] public bool PhoneIsValid { get; set; }
    [JsonPropertyName("isVerified")] public bool IsVerified { get; set; }
    [JsonPropertyName("defaultLanguage")] public string? DefaultLanguage { get; set; }
    [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("updatedAt")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("additionalData")] public Dictionary<string, string> AdditionalData { get; set; } = new();
    [JsonPropertyName("settingsData")] public Dictionary<string, string> SettingsData { get; set; } = new();
    
}