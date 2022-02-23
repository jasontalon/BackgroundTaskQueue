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
    private readonly ITaskQueue _taskQueue;


    public CustomersController(IMediator mediator, ITaskQueue taskQueue)
    {
        _mediator = mediator;
        _taskQueue = taskQueue;
    }

    [HttpPost]
    public async Task<Guid> CreateOrder(CreateCustomerCommand command)
    {
        var customerId = await _mediator.Send(command);

        await _taskQueue.EnqueueAsync(new CustomerCreatedNotification(customerId));

        return customerId;
    }
}