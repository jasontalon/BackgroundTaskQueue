using BackgroundTaskQueue.Handlers.Commands;
using BackgroundTaskQueue.Handlers.Notifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundTaskQueue.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;
    private readonly Dispatch _dispatch;

    public CustomersController(IMediator mediator, ITaskQueue TaskQueue,
        ILogger<CustomersController> logger, Dispatch dispatch)
    {
        _mediator = mediator;
        _logger = logger;
        _dispatch = dispatch;
    }

    [HttpPost]
    public async Task<Guid> CreateOrder(CreateCustomerCommand command)
    {
        var customerId = await _mediator.Send(command);

        //await _TaskQueue.QueueNotification(new CustomerCreatedNotification
        //    (customerId));

        await _dispatch(new CustomerCreatedNotification());
        return customerId;
    }
}