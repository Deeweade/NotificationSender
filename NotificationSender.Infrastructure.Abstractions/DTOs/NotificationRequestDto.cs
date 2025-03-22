namespace NotificationSender.Infrastructure.Abstractions.DTOs;

public class NotificationRequestDto : BaseDto
{
    public int SystemEventId { get; set; }
    public int NotificationChannelId { get; set; }
    public string RecipientAddress { get; set; }
    public Dictionary<string, string> Payload { get; set; }
    public bool RedirectNotifications { get; set; }
    public string RedirectAddress { get; set; }
    public DateTime CreatedAt { get; set; }

    public SystemEventDto SystemEvent { get; set; }
    public NotificationChannelDto NotificationChannel { get; set; }
    
    public ICollection<SentNotificationDto> SentNotifications { get; set; }
}