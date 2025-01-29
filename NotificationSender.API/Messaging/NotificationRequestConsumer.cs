using NotificationSender.Application.Abstractions.Commands;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using MediatR;

namespace NotificationSender.API.Messaging;

public class NotificationRequestConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IModel _channel;

    public NotificationRequestConsumer(IServiceProvider serviceProvider, IModel channel)
    {
        _serviceProvider = serviceProvider;
        _channel = channel;

        _channel.QueueDeclare(queue: "NotificationSenderQueue", durable: true, exclusive: false, autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());

            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var notificationRequest = JsonConvert.DeserializeObject<NotificationRequestCommand>(message);
            if (notificationRequest != null)
            {
                await mediator.Send(notificationRequest, stoppingToken);
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: "NotificationSenderQueue", autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
}