using NotificationSender.Application.Abstractions.Commands;
using NotificationSender.API.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using MediatR;

namespace NotificationSender.API.Messaging;

public class NotificationRequestConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public NotificationRequestConsumer(IServiceProvider serviceProvider, IOptions<RabbitMQOptions> rabbitOptions)
    {
        _serviceProvider = serviceProvider;
        var options = rabbitOptions.Value;

        var factory = new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Объявляем очередь. Если очередь уже существует, параметры должны совпадать.
        _channel.QueueDeclare(
            queue: "NotificationSenderQueue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
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

            // Подтверждаем обработку сообщения.
            _channel.BasicAck(ea.DeliveryTag, multiple: false);
        };

        // Запускаем потребителя на очереди с autoAck отключённым.
        _channel.BasicConsume(
            queue: "NotificationSenderQueue",
            autoAck: false,
            consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}