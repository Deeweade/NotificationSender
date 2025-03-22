using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Enums;

namespace NotificationSender.Application.Services;

public class SmsNotificationChannel : INotificationChannel
{
    private readonly INotificationTemplateProcessor _templateProcessor;
    private readonly IUnitOfWork _unitOfWork;

    public SmsNotificationChannel(INotificationTemplateProcessor templateProcessor, IUnitOfWork unitOfWork)
    {
        _templateProcessor = templateProcessor;
        _unitOfWork = unitOfWork;
    }

    public NotificationChannels ChannelType
    {
        get => NotificationChannels.Sms;
    }

    public async Task<SentNotificationDto> SendAsync(
        NotificationRequestDto request, 
        NotificationTemplateDto template, 
        CancellationToken cancellationToken)
    {
        var message = _templateProcessor.ProcessTemplate(template.Body, request.Payload);

        Console.WriteLine($"Sending SMS to {request.RecipientAddress} with message: {message}");

        await Task.CompletedTask;

        var sentNotification = new SentNotificationDto
        {
            NotificationRequestId = request.Id,
            NotificationStatusId = (int)NotificationStatuses.Sent,
            NotificationChannelId = (int)NotificationChannels.Email,
            SenderAddress = template.SystemEvent.ClientSystem.DefaultSenderEmail,
            UsedRecipientAddress = request.RecipientAddress,
            Body = message,
            CreatedAt = DateTime.Now
        };

        return await _unitOfWork.SentNotifications.AddAsync(sentNotification);
    }
}