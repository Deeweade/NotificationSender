using NotificationSender.Application.Abstractions.Commands;
using MassTransit;
using MediatR;

namespace NotificationSender.API.Messaging;

public class NotificationRequestCreatedConsumer : IConsumer<NotificationRequestCreatedCommand>
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationRequestCreatedConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Consume(ConsumeContext<NotificationRequestCreatedCommand> context)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        if (context.Message != null)
        {
            await mediator.Send(context.Message, CancellationToken.None);
        }
    }
}