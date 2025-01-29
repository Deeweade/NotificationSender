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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}