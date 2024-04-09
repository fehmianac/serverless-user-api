using System.Net;
using System.Text.Json;
using Amazon.SimpleNotificationService;
using Domain.Dto;
using Domain.Entities;
using Domain.Options;
using Domain.Services;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class EventBusManager : IEventBusManager
{
    private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
    private readonly IOptionsSnapshot<EventBusSettings> _eventBusSettingsOptions;

    public EventBusManager(IAmazonSimpleNotificationService amazonSimpleNotificationService, IOptionsSnapshot<EventBusSettings> eventBusSettingsOptions)
    {
        _amazonSimpleNotificationService = amazonSimpleNotificationService;
        _eventBusSettingsOptions = eventBusSettingsOptions;
    }

    public async Task<bool> UserModifiedAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("UserModified", user), cancellationToken);
    }

    public async Task<bool> OtpVerifiedAsync(string userId, string verifiedField, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("UserOtpVerified", new {UserId = userId, VerifiedField = verifiedField}), cancellationToken);
    }

    public async Task<bool> DeviceAddedAsync(UserDeviceEntity userDevice, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("UserDeviceAdded", userDevice), cancellationToken);
    }

    public async Task<bool> DeviceRemovedAsync(UserDeviceEntity userDevice, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("UserDeviceDeleted", userDevice), cancellationToken);
    }

    public async Task<bool> UserDeletedAsync(string userId, CancellationToken cancellationToken)
    {
        return await PublishAsync(new EventModel<object>("UserDeleted", new {UserId = userId}), cancellationToken);
    }

    public async Task<bool> IdentityVerifiedAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("UserIdentityVerified", new {UserId = userId}), cancellationToken);
    }

    public async Task<bool> EmailValidationOtpRequestAsync(string userId, string emailCode, CancellationToken cancellationToken)
    {
        return await PublishAsync(new EventModel<object>("UserEmailValidationOtpRequested", new {UserId = userId, EmailCode = emailCode}), cancellationToken);
    }

    public async Task<bool> PhoneValidationOtpRequestedAsync(string userId, string smsCode, CancellationToken cancellationToken)
    {
        return await PublishAsync(new EventModel<object>("UserPhoneValidationOtpRequested", new {UserId = userId, SmsCode = smsCode}), cancellationToken);
    }

    public async Task<bool> EmailUpdateOtpRequestedAsync(string userId, string newKey, string code, CancellationToken cancellationToken)
    {
        return await PublishAsync(new EventModel<object>("UserEmailUpdateOtpRequested", new {UserId = userId, NewEmail = newKey, EmailCode = code}), cancellationToken);
    }

    public async Task<bool> PhoneUpdateOtpRequestedAsync(string userId, string newKey, string code, CancellationToken cancellationToken)
    {
        return await PublishAsync(new EventModel<object>("UserPhoneUpdateOtpRequested", new {UserId = userId, NewPhone = newKey, SmsCode = code}), cancellationToken);
    }

    public async Task<bool> UserHasBeenSuspendedAsync(string userId, string requestReasonId, CancellationToken cancellationToken)
    {
        return await PublishAsync(new EventModel<object>("UserSuspended", new {UserId = userId}), cancellationToken);
    }

    private async Task<bool> PublishAsync(EventModel<object> eventModel, CancellationToken cancellationToken = default)
    {
        if (!_eventBusSettingsOptions.Value.IsEnabled)
            return true;

        var message = JsonSerializer.Serialize(eventModel);
        var snsResponse = await _amazonSimpleNotificationService.PublishAsync(_eventBusSettingsOptions.Value.TopicArn, message, cancellationToken);
        return snsResponse.HttpStatusCode is HttpStatusCode.OK or HttpStatusCode.Accepted or HttpStatusCode.Created;
    }
}