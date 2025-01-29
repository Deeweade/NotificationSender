namespace NotificationSender.Infrastructure.Abstractions.Repositories;

public interface IUnitOfWork : IDisposable
{
    INotificationTemplateRepository NotificationTemplates { get; }
    INotificationChannelRepository NotificationChannels { get; }
    INotificationRequestRepository NotificationRequests { get; }
    ISentNotificationRepository SentNotifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}