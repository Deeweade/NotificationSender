namespace NotificationSender.Infrastructure.Abstractions.DTOs;

public class NotificationStatusDto : BaseDto
{
    public string Name { get; set; }

    public ICollection<SentNotificationDto> SendNotifications { get; set; }
}