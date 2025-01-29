using NotificationSender.Domain.Enums;

namespace NotificationSender.Application.Abstractions.Services;

public interface INotificationChannelProvider
{
    INotificationChannel GetByType(NotificationChannels channelType);
}