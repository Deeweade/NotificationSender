namespace NotificationSender.Infrastructure.Abstractions.DTOs;

public class SystemEventDto : BaseDto
{
    public string EventName { get; set; }
    public int ClientSystemId { get; set; }

    public ConsumerSystemDto ClientSystem { get; set; }

    public ICollection<NotificationRequestDto> NotificationRequests { get; set; }
    public ICollection<NotificationTemplateDto> NotificationTemplates { get; set; }
}
