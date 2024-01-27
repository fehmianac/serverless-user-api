using System.Collections.ObjectModel;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Domain.Entities;
using Domain.Options;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class UserIdentityVerificationService : IUserIdentityVerificationService
{
    private readonly ReadOnlyCollection<string> _identityLabels =
        new(new[] { "Id Cards", "Passport", "Driving License" });


    private readonly IAmazonRekognition _amazonRekognition;
    private readonly IOptionsSnapshot<VerificationS3Settings> _verificationS3SettingsOptions;
    private readonly IUserRepository _userRepository;

    public UserIdentityVerificationService(IAmazonRekognition amazonRekognition,
        IOptionsSnapshot<VerificationS3Settings> verificationS3SettingsOptions, IUserRepository userRepository)
    {
        _amazonRekognition = amazonRekognition;
        _verificationS3SettingsOptions = verificationS3SettingsOptions;
        _userRepository = userRepository;
    }

    public async Task<(bool, List<string>)> CheckIsValidIdentityAsync(string idCardUrl,
        CancellationToken cancellationToken)
    {
        var objectKey = GetObjectKey(idCardUrl);
        var request = new DetectLabelsRequest
        {
            Image = new Image
            {
                S3Object = new S3Object
                {
                    Bucket = _verificationS3SettingsOptions.Value.BucketName,

                    Name = objectKey
                }
            }
        };
        var response = await _amazonRekognition.DetectLabelsAsync(request, cancellationToken);
        var labels = response.Labels.Where(q => q.Confidence > _verificationS3SettingsOptions.Value.MinConfidence)
            .Select(x => x.Name).ToList();
        var isIdentity = labels.Any(x => _identityLabels.Contains(x));
        return (isIdentity, response.Labels.Select(q => q.Name).ToList());
    }

    public async Task<bool> CompareFaceAndIdCardAsync(string userId, string faceUrl, string idCardUrl,
        CancellationToken cancellationToken)
    {
        var faceObjectKey = GetObjectKey(faceUrl);
        var idCardObjectKey = GetObjectKey(idCardUrl);

        var request = new CompareFacesRequest
        {
            SourceImage = new Image
            {
                S3Object = new S3Object
                {
                    Bucket = _verificationS3SettingsOptions.Value.BucketName,
                    Name = idCardObjectKey
                }
            },
            TargetImage = new Image
            {
                S3Object = new S3Object
                {
                    Bucket = _verificationS3SettingsOptions.Value.BucketName,
                    Name = faceObjectKey
                }
            },
        };
        var response = await _amazonRekognition.CompareFacesAsync(request, cancellationToken);
        var result = response.FaceMatches.Any(x => x.Similarity > _verificationS3SettingsOptions.Value.MinConfidence);
        if (!result)
            return result;

        var user = await _userRepository.GetAsync(userId, cancellationToken);
        if (user == null)
            return result;
        user.IsVerified = true;

        await _userRepository.SaveAsync(user, cancellationToken);
        return result;
    }

    public async Task<bool> VerifyByAvatarAsync(UserEntity user, string selfieUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(user.AvatarUrl))
            return false;
        var faceObjectKey = GetObjectKey(user.AvatarUrl);
        var selfieObjectKey = GetObjectKey(selfieUrl);

        var request = new CompareFacesRequest
        {
            SourceImage = new Image
            {
                S3Object = new S3Object
                {
                    Bucket = _verificationS3SettingsOptions.Value.BucketName,
                    Name = selfieObjectKey
                }
            },
            TargetImage = new Image
            {
                S3Object = new S3Object
                {
                    Bucket = _verificationS3SettingsOptions.Value.BucketName,
                    Name = faceObjectKey
                }
            },
        };
        var response = await _amazonRekognition.CompareFacesAsync(request, cancellationToken);
        var result = response.FaceMatches.Any(x => x.Similarity > _verificationS3SettingsOptions.Value.MinConfidence);
        if (!result)
            return result;
        user.IsVerified = true;
        user.SelfieUrl = selfieUrl;
        await _userRepository.SaveAsync(user, cancellationToken);
        return result;
    }

    private static string GetObjectKey(string url)
    {
        var uri = new Uri(url);
        return uri.AbsolutePath[1..];
    }
}