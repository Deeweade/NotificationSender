using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Entities;

namespace NotificationSender.Infrastructure.Abstractions.Repositories;

public interface INotificationRequestRepository : IRepository<NotificationRequest, NotificationRequestDto>
{

}