using BackgroundTaskQueue.Handlers.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using BackgroundTaskQueue.Handlers.Notifications;

namespace BackgroundTaskQueue.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITaskQueue _TaskQueue;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IMediator mediator, ITaskQueue TaskQueue,
        ILogger<CustomersController> logger)
    {
        _mediator = mediator;
        _TaskQueue = TaskQueue;
        _logger = logger;
    }

    [HttpPost]
    public async Task<Guid> CreateOrder(CreateCustomerCommand command)
    {
        var customerId = await _mediator.Send(command);
        
        await _TaskQueue.QueueNotification(new CustomerCreatedNotification
            {email = command.Email, customerId = customerId});

        return customerId;
    }
}