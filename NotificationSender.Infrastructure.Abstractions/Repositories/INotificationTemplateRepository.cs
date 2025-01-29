using NotificationSender.Infrastructure.Abstractions.QueryObjects;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Entities;

namespace NotificationSender.Infrastructure.Abstractions.Repositories;

public interface INotificationTemplateRepository :
    IRepository<NotificationTemplate, NotificationTemplateDto>, 
    IQueryRepository<NotificationTemplateDto, NotificationTemplateQuery>
{

}