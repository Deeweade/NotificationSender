using NotificationSender.Application.Abstractions.Services;
using System.Text.Json;

namespace NotificationSender.Application.Services;

public class NotificationTemplateProcessor : INotificationTemplateProcessor
{
    public string ProcessTemplate(string template, Dictionary<string, string> payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        
        try
        {
            foreach (var kvp in payload)
            {
                template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing payload: {ex.Message}");
            throw;
        }

        return template;
    }
}