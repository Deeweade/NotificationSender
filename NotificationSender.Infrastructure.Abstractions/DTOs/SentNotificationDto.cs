namespace NotificationSender.Infrastructure.Abstractions.DTOs;

public class SentNotificationDto : BaseDto
{
    public int NotificationStatusId { get; set; }
    public int NotificationRequestId { get; set; }
    public int NotificationChannelId { get; set; }
    public string UsedRecipientAddress { get; set; }
    public string SenderAddress { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }

    public NotificationStatusDto NotificationStatus { get; set; }
    public NotificationRequestDto NotificationRequest { get; set; }
    public NotificationChannelDto NotificationChannel { get; set; }
}