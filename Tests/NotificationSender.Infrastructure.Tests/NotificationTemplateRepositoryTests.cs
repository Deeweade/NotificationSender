using NotificationSender.Infrastructure.Abstractions.QueryObjects;
using NotificationSender.Infrastructure.Persistence.Repositories;
using NotificationSender.Infrastructure.Abstractions.DTOs;
using NotificationSender.Infrastructure.Contexts;
using NotificationSender.Infrastructure.Mappings;
using NotificationSender.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace NotificationSender.Infrastructure.Tests;

public class NotificationTemplateRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly IMapper _mapper;

    public NotificationTemplateRepositoryTests()
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
    public async Task GetSingleByQuery_ShouldReturnTemplate_WhenTemplateMatchesQuery()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new NotificationTemplateRepository(context, _mapper);

        var template = new NotificationTemplate
        {
            SystemEventId = 1,
            NotificationChannelId = 1,
            Subject = "Test Subject",
            Body = "Test Body"
        };

        await context.NotificationTemplates.AddAsync(template);
        await context.SaveChangesAsync();

        var query = new NotificationTemplateQuery
        {
            SystemEventIds = new List<int> { 1 },
            ChannelTypes = new List<int> { 1 }
        };

        // Act
        var result = await repository.GetSingleByQuery<NotificationTemplateDto>(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(template.Subject, result.Subject);
        Assert.Equal(template.Body, result.Body);
    }

    [Fact]
    public async Task GetSingleByQuery_ShouldReturnNull_WhenNoTemplateMatchesQuery()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new NotificationTemplateRepository(context, _mapper);

        var template = new NotificationTemplate
        {
            SystemEventId = 1,
            NotificationChannelId = 1,
            Subject = "Test Subject",
            Body = "Test Body"
        };

        await context.NotificationTemplates.AddAsync(template);
        await context.SaveChangesAsync();

        var query = new NotificationTemplateQuery
        {
            SystemEventIds = new List<int> { 2 }, // ID не совпадает
            ChannelTypes = new List<int> { 1 }
        };

        // Act
        var result = await repository.GetSingleByQuery<NotificationTemplateDto>(query);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCollectionByQuery_ShouldReturnTemplates_WhenTemplatesMatchQuery()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new NotificationTemplateRepository(context, _mapper);

        var templates = new List<NotificationTemplate>
        {
            new NotificationTemplate
            {
                SystemEventId = 1,
                NotificationChannelId = 1,
                Subject = "Subject 1",
                Body = "Body 1"
            },
            new NotificationTemplate
            {
                SystemEventId = 2,
                NotificationChannelId = 1,
                Subject = "Subject 2",
                Body = "Body 2"
            }
        };

        await context.NotificationTemplates.AddRangeAsync(templates);
        await context.SaveChangesAsync();

        var query = new NotificationTemplateQuery
        {
            SystemEventIds = new List<int> { 1, 2 },
            ChannelTypes = new List<int> { 1 }
        };

        // Act
        var result = await repository.GetCollectionByQuery<NotificationTemplateDto>(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Subject == "Subject 1");
        Assert.Contains(result, r => r.Subject == "Subject 2");
    }

    [Fact]
    public async Task GetCollectionByQuery_ShouldReturnEmptyCollection_WhenNoTemplatesMatchQuery()
    {
        // Arrange
        await using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new NotificationTemplateRepository(context, _mapper);

        var templates = new List<NotificationTemplate>
        {
            new NotificationTemplate
            {
                SystemEventId = 1,
                NotificationChannelId = 1,
                Subject = "Subject 1",
                Body = "Body 1"
            },
            new NotificationTemplate
            {
                SystemEventId = 2,
                NotificationChannelId = 1,
                Subject = "Subject 2",
                Body = "Body 2"
            }
        };

        await context.NotificationTemplates.AddRangeAsync(templates);
        await context.SaveChangesAsync();

        var query = new NotificationTemplateQuery
        {
            SystemEventIds = new List<int> { 3 }, // Не совпадает SystemEventId
            ChannelTypes = new List<int> { 2 } // Не совпадает NotificationChannelId
        };

        // Act
        var result = await repository.GetCollectionByQuery<NotificationTemplateDto>(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result); // Коллекция должна быть пустой
    }
}