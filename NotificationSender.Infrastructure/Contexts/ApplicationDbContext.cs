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

        base.OnModelCreating(modelBuilder);
    }
}