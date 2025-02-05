using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Infrastructure.Persistence.Repositories;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Persistence;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Infrastructure.Mappings;
using NotificationSender.Application.Commands;
using NotificationSender.Application.Services;
using NotificationSender.API.Messaging;
using NotificationSender.API.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

#region ControllersConfiguring

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

#endregion

string connectionString = builder.Configuration.GetConnectionString("NotificationSender");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("NotificationSender.API")));

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new InfrastructureMappingProfile());
});

#region DependenciesInjection

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
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddScoped<INotificationTemplateProcessor, NotificationTemplateProcessor>();
builder.Services.AddScoped<INotificationChannelProvider, NotificationChannelProvider>();
builder.Services.AddScoped<INotificationChannel, EmailNotificationChannel>();
builder.Services.AddScoped<INotificationChannel, SmsNotificationChannel>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
builder.Services.AddScoped<INotificationChannelRepository, NotificationChannelRepository>();
builder.Services.AddScoped<INotificationRequestRepository, NotificationRequestRepository>();
builder.Services.AddScoped<ISentNotificationRepository, SentNotificationRepository>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

#endregion

#region OtherSettings

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationSender API", Version = "v1" });
});

var corsPolicyName = "AllowCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true);
    });
});

#endregion

var app = builder.Build();

#region ApplicationSettingUp

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});
app.UseRouting();

app.UseCors(corsPolicyName);

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

#region RunMigrations

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

#endregion

app.Run();