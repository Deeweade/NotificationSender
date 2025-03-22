using NotificationSender.Infrastructure.Abstractions.QueryObjects;
using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace NotificationSender.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationChannelProvider _channelProvider;
    private readonly ILogger<NotificationService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork, 
        INotificationChannelProvider channelProvider, 
        ILogger<NotificationService> logger)
    {
        _channelProvider = channelProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SendNotificationAsync(NotificationRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var template = await _unitOfWork.NotificationTemplates.GetSingleByQuery<NotificationTemplateDto>(
                new NotificationTemplateQuery
            {
                SystemEventIds = new List<int> { request.SystemEventId },
                ChannelTypes = new List<int> { request.NotificationChannelId }
            });

            if (template is null) throw new InvalidOperationException("Notification template not found.");

            var channel = _channelProvider.GetByType((NotificationChannels)request.NotificationChannelId);
            
            await channel.SendAsync(request, template, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}