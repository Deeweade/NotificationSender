using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Enums;
using MailKit.Net.Smtp;
using MimeKit;

namespace NotificationSender.Application.Services;

public class EmailNotificationChannel : INotificationChannel
{
    private readonly INotificationTemplateProcessor _templateProcessor;
    private readonly IUnitOfWork _unitOfWork;

    public EmailNotificationChannel(INotificationTemplateProcessor templateProcessor, 
        IUnitOfWork unitOfWork)
    {
        _templateProcessor = templateProcessor;
        _unitOfWork = unitOfWork;
    }

    public NotificationChannels ChannelType
    {
        get => NotificationChannels.Mail;
    }

    public async Task<SentNotificationDto> SendAsync(
        NotificationRequestDto request, 
        NotificationTemplateDto template, 
        CancellationToken cancellationToken)
    {
        // Обработка шаблона и вставка значений плейсхолдеров
        var body = _templateProcessor.ProcessTemplate(template.Body, request.Payload);
        var subject = template.Subject;
        var recipientAddress = request.RedirectNotifications
            ? !string.IsNullOrEmpty(request.RecipientAddress)
                ? request.RedirectAddress
                : template.SystemEvent.ClientSystem.DefaultRedirectEmail
            : request.RecipientAddress;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(template.SystemEvent.ClientSystem.DefaultDisplayName, template.SystemEvent.ClientSystem.DefaultSenderEmail));
        message.To.Add(new MailboxAddress(recipientAddress, recipientAddress));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { TextBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("mailhog", 1025, false, cancellationToken);
            //await client.AuthenticateAsync("your-email@example.com", "your-email-password", cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }

        var sentNotification = new SentNotificationDto
        {
            NotificationRequestId = request.Id,
            NotificationStatusId = (int)NotificationStatuses.Sent,
            NotificationChannelId = (int)NotificationChannels.Mail,
            SenderAddress = template.SystemEvent.ClientSystem.DefaultSenderEmail,
            UsedRecipientAddress = recipientAddress,
            Subject = subject,
            Body = body,
            CreatedAt = DateTime.Now
        };

        return await _unitOfWork.SentNotifications.AddAsync(sentNotification);
    }
}