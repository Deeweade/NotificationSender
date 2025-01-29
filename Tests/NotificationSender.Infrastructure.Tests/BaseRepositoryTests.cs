using NotificationSender.Infrastructure.Persistence.Repositories;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Infrastructure.Mappings;
using NotificationSender.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace NotificationSender.Infrastructure.Tests;

public class BaseRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly IMapper _mapper;

    public BaseRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .Options;

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new InfrastructureMappingProfile());
        });

        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new BaseRepository<NotificationChannel, NotificationChannelDto>(context, _mapper);

        var channelDto = new NotificationChannelDto
        {
            Id = 1,
            Name = "Email",
            IsEnabled = true
        };

        // Act
        await repository.AddAsync(channelDto);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Email", result?.Name);
        Assert.True(result?.IsEnabled);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new BaseRepository<NotificationChannel, NotificationChannelDto>(context, _mapper);

        var channel1 = new NotificationChannel { Id = 1, Name = "Email", IsEnabled = true };
        var channel2 = new NotificationChannel { Id = 2, Name = "SMS", IsEnabled = false };

        context.NotificationChannels.Add(channel1);
        context.NotificationChannels.Add(channel2);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Update_ShouldModifyEntity()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new BaseRepository<NotificationChannel, NotificationChannelDto>(context, _mapper);

        var channel = new NotificationChannel { Id = 3, Name = "Email", IsEnabled = true };
        context.NotificationChannels.Add(channel);
        await context.SaveChangesAsync();

        var updatedDto = new NotificationChannelDto { Id = 3, Name = "Push", IsEnabled = false };

        // Act
        await repository.Update(updatedDto);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Push", result?.Name);
        Assert.False(result?.IsEnabled);
    }

    [Fact]
    public async Task Remove_ShouldDeleteEntity()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new BaseRepository<NotificationChannel, NotificationChannelDto>(context, _mapper);

        var channel = new NotificationChannel { Id = 4, Name = "Email", IsEnabled = true };
        context.NotificationChannels.Add(channel);
        await context.SaveChangesAsync();

        var dtoToRemove = new NotificationChannelDto { Id = 4, Name = "Email", IsEnabled = true };

        // Act
        await repository.Remove(dtoToRemove);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(4);

        // Assert
        Assert.Null(result);
    }
}