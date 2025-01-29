using NotificationSender.Infrastructure.Persistence.Repositories;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Infrastructure.Persistence;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Infrastructure.Mappings;
using NotificationSender.Application.Services;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Moq;

namespace NotificationSender.Application.Tests;

public class EmailNotificationChannelTests
{
    private readonly Mock<INotificationTemplateProcessor> _mockTemplateProcessor;
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly IMapper _mapper;

    public EmailNotificationChannelTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Уникальная база данных для каждого теста
            .Options;

        _mockTemplateProcessor = new Mock<INotificationTemplateProcessor>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new InfrastructureMappingProfile());
        });

        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task SendAsync_ShouldSaveSentNotificationToDatabase()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_dbContextOptions);
        var unitOfWork = new UnitOfWork(
            context,
            new NotificationTemplateRepository(context, _mapper),
            new NotificationChannelRepository(context, _mapper),
            new NotificationRequestRepository(context, _mapper),
            new SentNotificationRepository(context, _mapper)
        );

        var emailChannel = new EmailNotificationChannel(_mockTemplateProcessor.Object, unitOfWork);

        var request = new NotificationRequestDto
        {
            Id = 1,
            RecipientAddress = "user@example.com",
            Payload = "{ \"name\": \"Test User\" }",
            RedirectNotifications = false
        };

        var template = new NotificationTemplateDto
        {
            Subject = "Hello, {{name}}!",
            Body = "Welcome to our service, {{name}}.",
            SystemEvent = new SystemEventDto
            {
                ClientSystem = new ConsumerSystemDto
                {
                    DefaultSenderEmail = "noreply@example.com",
                    DefaultDisplayName = "Notification Service"
                }
            }
        };

        _mockTemplateProcessor
            .Setup(tp => tp.ProcessTemplate(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("Hello, Test User!");

        // Act
        var sentNotification = await emailChannel.SendAsync(request, template, CancellationToken.None);

        // Assert
        var result = await unitOfWork.SentNotifications.GetByIdAsync(sentNotification.Id);

        Assert.NotNull(result);
        Assert.Equal("Hello, Test User!", result.Body);
        Assert.Equal("noreply@example.com", result.SenderAddress);
        Assert.Equal("user@example.com", result.UsedRecipientAddress);
    }

    [Fact]
    public async Task SendAsync_ShouldRedirectNotification_WhenRedirectIsEnabled()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_dbContextOptions);
        var unitOfWork = new UnitOfWork(
            context,
            new NotificationTemplateRepository(context, _mapper),
            new NotificationChannelRepository(context, _mapper),
            new NotificationRequestRepository(context, _mapper),
            new SentNotificationRepository(context, _mapper)
        );

        var emailChannel = new EmailNotificationChannel(_mockTemplateProcessor.Object, unitOfWork);

        var request = new NotificationRequestDto
        {
            Id = 2,
            RedirectNotifications = true,
            RedirectAddress = "redirect@example.com",
            RecipientAddress = "user@example.com",
            Payload = "{ \"name\": \"Test User\" }"
        };

        var template = new NotificationTemplateDto
        {
            Subject = "Hello, {{name}}!",
            Body = "Welcome to our service, {{name}}.",
            SystemEvent = new SystemEventDto
            {
                ClientSystem = new ConsumerSystemDto
                {
                    DefaultSenderEmail = "noreply@example.com",
                    DefaultDisplayName = "Notification Service",
                    DefaultRedirectEmail = "default-redirect@example.com"
                }
            }
        };

        _mockTemplateProcessor
            .Setup(tp => tp.ProcessTemplate(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("Hello, Test User!");

        // Act
        var sentNotification = await emailChannel.SendAsync(request, template, CancellationToken.None);

        // Assert
        var result = await unitOfWork.SentNotifications.GetByIdAsync(sentNotification.Id);

        Assert.NotNull(result);
        Assert.Equal("redirect@example.com", result.UsedRecipientAddress);
        Assert.Equal("Hello, Test User!", result.Body);
    }
}