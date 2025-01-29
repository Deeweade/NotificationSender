using MediatR;

namespace NotificationSender.Application.Abstractions.Commands;

public record NotificationRequestCommand(
    int SystemEventId,
    int ChannelId, 
    string RecipientAddress,
    string Payload,
    bool RedirectNotifications,
    string RedirectAddress
) : IRequest<Unit>;