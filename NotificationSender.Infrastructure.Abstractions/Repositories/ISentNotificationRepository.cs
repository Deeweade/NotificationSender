using NotificationSender.Domain.Entities;
using NotificationSender.Infrastructure.Abstractions.DTOs;

namespace NotificationSender.Infrastructure.Abstractions.Repositories;

public interface ISentNotificationRepository : IRepository<SentNotification, SentNotificationDto>
{
}