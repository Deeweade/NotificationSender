using NotificationSender.Infrastructure.Abstractions.DTOs;

namespace NotificationSender.Application.Abstractions.Services;

public interface INotificationService
{
    Task SendNotificationAsync(NotificationRequestDto notificationRequestDto, CancellationToken cancellationToken);
}