using NotificationSender.Application.Abstractions.Services;
using System.Text.Json;

namespace NotificationSender.Application.Services;

public class NotificationTemplateProcessor : INotificationTemplateProcessor
{
    public string ProcessTemplate(string template, string payload)
    {
        if (string.IsNullOrEmpty(payload))
            return template;

        try
        {
            var payloadDict = JsonSerializer.Deserialize<Dictionary<string, string>>(payload);

            if (payloadDict != null)
            {
                foreach (var kvp in payloadDict)
                {
                    template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
                }
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