namespace NotificationSender.Domain.Entities;

public class NotificationRequest : BaseEntity
{
    public int SystemEventId { get; set; }
    public string ChannelType { get; set; }
    public string RecipientAddress { get; set; }
    public string Payload { get; set; }
    public bool RedirectNotifications { get; set; }
    public string RedirectAddress { get; set; }
    public DateTime CreatedAt { get; set; }

    public SystemEvent SystemEvent { get; set; }
    public ICollection<SentNotification> SentNotifications { get; set; }
}