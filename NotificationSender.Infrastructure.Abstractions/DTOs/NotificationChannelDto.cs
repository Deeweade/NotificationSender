namespace NotificationSender.Infrastructure.Abstractions.DTOs;

public class NotificationChannelDto : BaseDto
{
    public string Name { get; set; }
    public bool IsEnabled { get; set; }

    public ICollection<ConsumerSystemDto> ClientSystems { get; set; }
    public ICollection<NotificationTemplateDto> Templates { get; set; }
    public ICollection<SentNotificationDto> SentNotifications { get; set; }
}