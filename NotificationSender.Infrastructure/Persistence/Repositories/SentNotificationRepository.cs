using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Domain.Entities;
using AutoMapper;

namespace NotificationSender.Infrastructure.Persistence.Repositories;

public class SentNotificationRepository : BaseRepository<SentNotification, SentNotificationDto>,
    ISentNotificationRepository
{
    public SentNotificationRepository(ApplicationDbContext context, IMapper mapper) 
        : base(context, mapper)
    {
    }
}
