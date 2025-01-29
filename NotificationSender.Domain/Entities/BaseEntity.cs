using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationSender.Domain.Entities;

public abstract class BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
}
