namespace NotificationSender.Application.Abstractions.Services;

public interface INotificationTemplateProcessor
{
    string ProcessTemplate(string template, Dictionary<string, string> payload);
}