using NotificationSender.Infrastructure.Abstractions.QueryObjects;
using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Enums;

namespace NotificationSender.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationChannelProvider _channelProvider;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork, INotificationChannelProvider channelProvider)
    {
        _channelProvider = channelProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task SendNotificationAsync(NotificationRequestDto request, CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.NotificationTemplates.GetSingleByQuery<NotificationTemplateDto>(
            new NotificationTemplateQuery
        {
            SystemEventIds = new List<int> { request.SystemEventId },
            ChannelTypes = new List<int> { request.NotificationChannelId }
        });

        if (template is null) throw new InvalidOperationException("Notification template not found.");

        var channel = _channelProvider.GetByType((NotificationChannels)request.NotificationChannelId);

        // var channelDto = await _unitOfWork.NotificationChannels.GetByIdAsync(request.NotificationChannelId);

        // if (!channelDto.IsEnabled)
        // {
        //     throw new InvalidOperationException($"{channelDto.Name} notification channel is not enabled!");
        // }
        
        await channel.SendAsync(request, template, cancellationToken);
    }
}