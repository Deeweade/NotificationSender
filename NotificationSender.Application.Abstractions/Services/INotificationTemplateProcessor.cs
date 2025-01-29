namespace NotificationSender.Application.Abstractions.Services;

public interface INotificationTemplateProcessor
{
    string ProcessTemplate(string template, string payload);
}