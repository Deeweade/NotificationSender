using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Entities;
using AutoMapper;

namespace NotificationSender.Infrastructure.Mappings;

public class InfrastructureMappingProfile : Profile
{
    public InfrastructureMappingProfile()
    {
        CreateMap<NotificationTemplate, NotificationTemplateDto>().ReverseMap();
        CreateMap<NotificationChannel, NotificationChannelDto>().ReverseMap();
        CreateMap<NotificationRequest, NotificationRequestDto>().ReverseMap();
        CreateMap<NotificationStatus, NotificationStatusDto>().ReverseMap();
        CreateMap<SentNotification, SentNotificationDto>().ReverseMap();
        CreateMap<ConsumerSystem, ConsumerSystemDto>().ReverseMap();
        CreateMap<SystemEvent, SystemEventDto>().ReverseMap();
    }
}