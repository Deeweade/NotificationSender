namespace NotificationSender.Domain.Entities;

public class NotificationChannel : BaseEntity
{
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    
    public ICollection<ConsumerSystem> ClientSystems { get; set; }
    public ICollection<NotificationTemplate> Templates { get; set; }
    public ICollection<SentNotification> SentNotifications { get; set; }
}