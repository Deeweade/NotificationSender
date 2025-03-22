using MediatR;

namespace NotificationSender.Application.Abstractions.Commands;

public record NotificationRequestCreatedCommand(
    int SystemEventId,
    int ChannelId, 
    string RecipientAddress,
    Dictionary<string, string> Payload,
    bool RedirectNotifications,
    string RedirectAddress
) : IRequest<Unit>;