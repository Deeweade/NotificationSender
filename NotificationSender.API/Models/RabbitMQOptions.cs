namespace NotificationSender.API.Models;

public class RabbitMQOptions
{
    public int Port { get; set; }    
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}