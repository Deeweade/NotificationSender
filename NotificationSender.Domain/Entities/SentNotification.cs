namespace NotificationSender.Domain.Entities;

public class SentNotification : BaseEntity
{
    public int NotificaitonStatusId { get; set; }
    public int NotificationRequestId { get; set; }
    public int NotificationChannelId { get; set; }
    public string ChannelType { get; set; }
    public string UsedRecipientAddress { get; set; }
    public string SenderAddress { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }

    public NotificationStatus NotificationStatus { get; set; }
    public NotificationRequest NotificationRequest { get; set; }
    public NotificationChannel NotificationChannel { get; set; }
}