using NotificationSender.Infrastructure.Abstractions.Repositories;
using NotificationSender.Infrastructure.Persistence.Repositories;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Persistence;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Infrastructure.Mappings;
using NotificationSender.Application.Handlers;
using NotificationSender.Application.Services;
using NotificationSender.API.Middlewares;
using NotificationSender.API.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog.Sinks.Elasticsearch;
using Serilog;
using MassTransit;
using AutoMapper;

#region EnvironmentConfiguring

var builder = WebApplication.CreateBuilder();

var environmentName = builder.Environment.EnvironmentName;

var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Settings");

builder.Configuration
    .SetBasePath(settingsPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

#endregion

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

#region ContextsConfiguring

string connectionString = builder.Configuration.GetConnectionString("NotificationSender");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("NotificationSender.API")));

#endregion

#region DependenciesInjection

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(NotificationRequestHandler).Assembly);
});

var rabbitMqOptions = builder.Configuration.GetSection("RabbitMq").Get<ExternalServiceOptions>();

builder.Services.AddMassTransit(x =>
{
    var assembly = typeof(Program).Assembly;
    x.AddConsumers(assembly);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqOptions.HostName, (ushort)rabbitMqOptions.Port, "/", h =>
        {
            h.Username(rabbitMqOptions.UserName);
            h.Password(rabbitMqOptions.Password);
        });
        cfg.ConfigureEndpoints(context);
    });
});
// builder.Services.AddSingleton(sp =>
// {
//     var connectionFactory = new ConnectionFactory { HostName = "localhost" };
//     return connectionFactory.CreateConnection().CreateModel();
// });
// builder.Services.AddHostedService<NotificationRequestConsumer>();
// builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));

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

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new InfrastructureMappingProfile());
});

builder.Services.AddSingleton(mapperConfig.CreateMapper());

#endregion

#region Logs

var elkOptions = builder.Configuration.GetSection("ElasticSearch").Get<ExternalServiceOptions>();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri($"{elkOptions.HostName}:{elkOptions.Port}"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"dotnet-logs-{DateTime.Now:yyyy.MM.dd}",
        NumberOfReplicas = 1,
        NumberOfShards = 2
    })
    .CreateLogger();

builder.Host.UseSerilog();

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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();

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