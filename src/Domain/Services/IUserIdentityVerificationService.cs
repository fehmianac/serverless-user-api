using Domain.Entities;

namespace Domain.Services;

public interface IUserIdentityVerificationService
{
    Task<(bool, List<string>)> CheckIsValidIdentityAsync(string idCardUrl, CancellationToken cancellationToken);

    Task<bool> CompareFaceAndIdCardAsync(string userId, string faceUrl, string idCardUrl, CancellationToken cancellationToken);
    
    Task<bool> VerifyByAvatarAsync(UserEntity user,string selfieUrl, CancellationToken cancellationToken);
}