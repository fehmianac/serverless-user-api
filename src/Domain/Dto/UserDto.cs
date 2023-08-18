using Domain.Entities;

namespace Domain.Dto;

public class UserDto
{
    public string Id { get; set; } = default!;
    public string? UserName { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Status { get; set; } = default!;
    public bool EmailIsValid { get; set; }
    public bool PhoneIsValid { get; set; }
    public bool IsVerified { get; set; }
    public string? DefaultLanguage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> AdditionalData { get; set; } = new();
    public Dictionary<string, string> SettingsData { get; set; } = new();
}

public static class UserDtoMapper
{
    public static UserEntity ToEntity(this UserDto dto)
    {
        return new UserEntity
        {
            Email = dto.Email,
            Gender = dto.Gender,
            Id = dto.Id,
            Phone = dto.Phone,
            UserName = dto.UserName,
            AvatarUrl = dto.AvatarUrl,
            BirthDate = dto.BirthDate,
            DefaultLanguage = dto.DefaultLanguage,
            EmailIsValid = dto.EmailIsValid,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneIsValid = dto.PhoneIsValid,
            Status = dto.Status,
            IsVerified = dto.IsVerified,
            AdditionalData = dto.AdditionalData,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    public static UserDto ToDto(this UserEntity entity)
    {
        return new UserDto
        {
            Email = entity.Email,
            Gender = entity.Gender,
            Id = entity.Id,
            Phone = entity.Phone,
            UserName = entity.UserName,
            AvatarUrl = entity.AvatarUrl,
            BirthDate = entity.BirthDate,
            DefaultLanguage = entity.DefaultLanguage,
            EmailIsValid = entity.EmailIsValid,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            PhoneIsValid = entity.PhoneIsValid,
            Status = entity.Status,
            IsVerified = entity.IsVerified,
            AdditionalData = entity.AdditionalData,
            SettingsData = entity.SettingsData,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }
}