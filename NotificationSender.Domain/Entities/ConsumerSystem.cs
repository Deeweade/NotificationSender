namespace NotificationSender.Domain.Entities;

public class ConsumerSystem : BaseEntity
{
    public string SystemName { get; set; }
    public string DefaultDisplayName { get; set; }
    public string DefaultSenderEmail { get; set; }
    public string DefaultRedirectEmail { get; set; }
    public string DefaultSenderPhone { get; set; }
    public int DefaultNotificationChannelId { get; set; }

    public NotificationChannel DefaultNotificationChannel { get; set; }

    public ICollection<SystemEvent> SystemEvents { get; set; }
}