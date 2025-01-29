using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Domain.Enums;

namespace NotificationSender.Application.Services;

public class NotificationChannelProvider : INotificationChannelProvider
{
    private readonly IEnumerable<INotificationChannel> _channels;

    public NotificationChannelProvider(IEnumerable<INotificationChannel> channels)
    {
        _channels = channels;
    }

    public INotificationChannel GetByType(NotificationChannels channelType)
    {
        var channel = _channels.Where(x => x.ChannelType.Equals(channelType)).FirstOrDefault();

        if (channel is null) throw new NotSupportedException($"Channel type '{channelType}' is not supported.");

        return channel;
    }
}