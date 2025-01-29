using NotificationSender.Infrastructure.Persistence.Repositories;
using NotificationSender.Application.Abstractions.Services;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Infrastructure.Persistence;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Infrastructure.Mappings;
using NotificationSender.Application.Services;
using NotificationSender.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Moq;

namespace NotificationSender.Application.Tests;

public class NotificationServiceTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly Mock<INotificationChannelProvider> _mockChannelProvider;
    private readonly Mock<INotificationChannel> _mockNotificationChannel;
    private readonly IMapper _mapper;

    public NotificationServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _mockChannelProvider = new Mock<INotificationChannelProvider>();
        _mockNotificationChannel = new Mock<INotificationChannel>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new InfrastructureMappingProfile());
        });

        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldThrow_WhenTemplateNotFound()
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

        var notificationService = new NotificationService(unitOfWork, _mockChannelProvider.Object);

        var request = new NotificationRequestDto
        {
            SystemEventId = 1,
            NotificationChannelId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            notificationService.SendNotificationAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task SendNotificationAsync_ShouldCallChannelSend_WhenValidRequest()
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

        var notificationService = new NotificationService(unitOfWork, _mockChannelProvider.Object);

        var request = new NotificationRequestDto
        {
            SystemEventId = 1,
            NotificationChannelId = 1,
            RecipientAddress = "user@example.com"
        };

        var template = new NotificationTemplateDto
        {
            SystemEventId = 1,
            NotificationChannelId = 1,
            Subject = "Test Subject",
            Body = "Test Body"
        };

        _mockChannelProvider
            .Setup(cp => cp.GetByType((NotificationChannels)request.NotificationChannelId))
            .Returns(_mockNotificationChannel.Object);

        _mockNotificationChannel
            .Setup(c => c.SendAsync(It.IsAny<NotificationRequestDto>(), It.IsAny<NotificationTemplateDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SentNotificationDto());

        await unitOfWork.NotificationTemplates.AddAsync(template);

        // Act
        await notificationService.SendNotificationAsync(request, CancellationToken.None);

        // Assert
        _mockNotificationChannel.Verify(c => c.SendAsync(request, It.IsAny<NotificationTemplateDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}