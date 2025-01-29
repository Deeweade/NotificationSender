using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Persistence;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Infrastructure.Mappings;
using NotificationSender.Application.Commands;
using NotificationSender.Application.Services;
using NotificationSender.API.Messaging;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using AutoMapper;
using NotificationSender.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new InfrastructureMappingProfile());
});

builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(NotificationRequestHandler).Assembly);
});
builder.Services.AddSingleton(sp =>
{
    var connectionFactory = new ConnectionFactory { HostName = "localhost" };
    return connectionFactory.CreateConnection().CreateModel();
});
builder.Services.AddHostedService<NotificationRequestConsumer>();

builder.Services.AddSingleton<INotificationChannelProvider, NotificationChannelProvider>();
builder.Services.AddScoped<INotificationTemplateProcessor, NotificationTemplateProcessor>();
builder.Services.AddScoped<INotificationChannel, EmailNotificationChannel>();
builder.Services.AddScoped<INotificationChannel, SmsNotificationChannel>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.Run();