namespace NotificationSender.Infrastructure.Abstractions.DTOs;

public class NotificationTemplateDto : BaseDto
{
    public int SystemEventId { get; set; }
    public int NotificationChannelId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public SystemEventDto SystemEvent { get; set; }
    public NotificationChannelDto NotificationChannel { get; set; }
}