using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Infrastructure.Contexts;

namespace NotificationSender.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context,
        INotificationTemplateRepository notificationTemplates,
        INotificationChannelRepository notificationChannels,
        INotificationRequestRepository notificationRequests,
        ISentNotificationRepository sentNotifications)
    {
        _context = context;

        NotificationTemplates = notificationTemplates;
        NotificationChannels = notificationChannels;
        NotificationRequests = notificationRequests;
        SentNotifications = sentNotifications;
    }

    public INotificationTemplateRepository NotificationTemplates { get; }
    public INotificationChannelRepository NotificationChannels { get; }
    public INotificationRequestRepository NotificationRequests { get; }
    public ISentNotificationRepository SentNotifications { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}