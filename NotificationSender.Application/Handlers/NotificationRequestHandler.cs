using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Application.Abstractions.Commands;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using MediatR;

namespace NotificationSender.Application.Handlers;

public class NotificationRequestHandler : IRequestHandler<NotificationRequestCreatedCommand, Unit>
{
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationRequestHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(NotificationRequestCreatedCommand command, CancellationToken cancellationToken)
    {
        var request = new NotificationRequestDto
        {
            SystemEventId = command.SystemEventId,
            NotificationChannelId = command.ChannelId,
            RecipientAddress = command.RecipientAddress,
            Payload = command.Payload,
            RedirectNotifications = command.RedirectNotifications,
            RedirectAddress = command.RedirectAddress,
            CreatedAt = DateTime.Now
        };

        request = await _unitOfWork.NotificationRequests.AddAsync(request);

        await _notificationService.SendNotificationAsync(request, cancellationToken);

        return Unit.Value;
    }
}