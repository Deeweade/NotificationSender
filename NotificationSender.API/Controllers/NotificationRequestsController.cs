using NotificationSender.Application.Abstractions.Commands;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace NotificationSender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationRequestsController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationRequestsController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public async Task<IActionResult> Create(NotificationRequestCreatedCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(command, CancellationToken.None);

            return Ok();
        }
    }
}