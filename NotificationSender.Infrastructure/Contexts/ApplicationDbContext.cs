using NotificationSender.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace NotificationSender.Infrastructure.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    public DbSet<SystemEvent> SystemEvents { get; set; }
    public DbSet<ConsumerSystem> ClientSystems { get; set; }
    public DbSet<SentNotification> SentNotifications { get; set; }
    public DbSet<NotificationChannel> NotificationChannels { get; set; }
    public DbSet<NotificationRequest> NotificationRequests { get; set; }
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationChannel>()
            .HasMany(x => x.ClientSystems)
            .WithOne(x => x.DefaultNotificationChannel)
            .HasForeignKey(x => x.DefaultNotificationChannelId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<NotificationChannel>()
            .HasMany(x => x.SentNotifications)
            .WithOne(x => x.NotificationChannel)
            .HasForeignKey(x => x.NotificationChannelId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<NotificationChannel>()
            .HasMany(x => x.Templates)
            .WithOne(x => x.NotificationChannel)
            .HasForeignKey(x => x.NotificationChannelId)
            .OnDelete(DeleteBehavior.NoAction);

        Seed(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationChannel>().HasData(
            new NotificationChannel { Id = 1, Name = "Mail", IsEnabled = true },
            new NotificationChannel { Id = 2, Name = "SMS", IsEnabled = false }
        );

        modelBuilder.Entity<ConsumerSystem>().HasData(
            new ConsumerSystem 
            { 
                Id = 1, 
                SystemName = "Absence", 
                DefaultDisplayName = "Система отпусков", 
                DefaultSenderEmail = "email@gmail.com", 
                DefaultNotificationChannelId = 1 
            },
            new ConsumerSystem 
            { 
                Id = 2, 
                SystemName = "My goals", 
                DefaultDisplayName = "Мои цели", 
                DefaultSenderEmail = "email@gmail.com", 
                DefaultNotificationChannelId = 1 
            }
        );

        modelBuilder.Entity<SystemEvent>().HasData(
            new SystemEvent { Id = 1, EventName = "Сотрудник направил отпуска на согласование", ConsumerSystemId = 1 },
            new SystemEvent { Id = 2, EventName = "Руководитель отклонил отпуск", ConsumerSystemId = 1 },
            new SystemEvent { Id = 3, EventName = "Руководитель согласовал отпуск", ConsumerSystemId = 1 },
            new SystemEvent { Id = 4, EventName = "Руководитель отклонил все отпуска", ConsumerSystemId = 1 },
            new SystemEvent { Id = 5, EventName = "Руководитель согласовал все отпуска", ConsumerSystemId = 1 }
        );
    }
}