namespace NotificationSender.Domain.Entities;

public class SystemEvent : BaseEntity
{
    public string EventName { get; set; }
    public int ConsumerSystemId { get; set; }

    public ConsumerSystem ConsumerSystem { get; set; }
    
    public ICollection<NotificationRequest> NotificationRequests { get; set; }
    public ICollection<NotificationTemplate> NotificationTemplates { get; set; }
}
