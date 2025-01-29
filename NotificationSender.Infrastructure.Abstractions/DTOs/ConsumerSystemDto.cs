namespace NotificationSender.Infrastructure.Abstractions.DTOs;

public class ConsumerSystemDto : BaseDto
{
    public string SystemName { get; set; }
    public int DefaultNotificationChannelId { get; set; }
    public string DefaultDisplayName { get; set; }
    public string DefaultSenderEmail { get; set; }
    public string DefaultRedirectEmail { get; set; }
    public string DefaultSenderPhone { get; set; }

    public NotificationChannelDto DefaultNotificationChannel { get; set; }

    public ICollection<SystemEventDto> SystemEvents { get; set; }
}
