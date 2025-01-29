namespace NotificationSender.Domain.Entities;

public class NotificationStatus : BaseEntity
{
    public string Name { get; set; }

    public ICollection<SentNotification> SendNotifications { get; set; }
}
