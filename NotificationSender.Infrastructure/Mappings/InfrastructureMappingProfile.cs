using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Domain.Entities;
using Newtonsoft.Json;
using AutoMapper;

namespace NotificationSender.Infrastructure.Mappings;

public class InfrastructureMappingProfile : Profile
{
    public InfrastructureMappingProfile()
    {
        CreateMap<NotificationTemplate, NotificationTemplateDto>().ReverseMap();
        CreateMap<NotificationChannel, NotificationChannelDto>().ReverseMap();
        CreateMap<NotificationStatus, NotificationStatusDto>().ReverseMap();
        CreateMap<SentNotification, SentNotificationDto>().ReverseMap();
        CreateMap<ConsumerSystem, ConsumerSystemDto>().ReverseMap();
        CreateMap<SystemEvent, SystemEventDto>().ReverseMap();

        CreateMap<NotificationRequestDto, NotificationRequest>()
            .ForMember(dest => dest.Payload, opt => opt.MapFrom(src =>
                src.Payload != null ? JsonConvert.SerializeObject(src.Payload) : null));
        
        CreateMap<NotificationRequest, NotificationRequestDto>()
            .ForMember(dest => dest.Payload, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.Payload) 
                    ? JsonConvert.DeserializeObject<Dictionary<string, string>>(src.Payload) 
                    : new Dictionary<string, string>()));
    }
}