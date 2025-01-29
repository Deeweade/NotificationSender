using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Enums;

namespace NotificationSender.Application.Abstractions.Services;

public interface INotificationChannel
{
    NotificationChannels ChannelType { get; }
    Task<SentNotificationDto> SendAsync(NotificationRequestDto request, NotificationTemplateDto template, CancellationToken cancellationToken);
}