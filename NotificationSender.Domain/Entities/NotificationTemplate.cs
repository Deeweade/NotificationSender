namespace NotificationSender.Domain.Entities;

public class NotificationTemplate : BaseEntity
{
    public int SystemEventId { get; set; }
    public int NotificationChannelId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public SystemEvent SystemEvent { get; set; }
    public NotificationChannel NotificationChannel { get; set; }
}