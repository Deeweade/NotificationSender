namespace NotificationSender.Infrastructure.Abstractions.QueryObjects;

public class NotificationTemplateQuery : QueryObject
{
    public List<int> ChannelTypes { get; set; }
    public List<int> SystemEventIds { get; set; }
}