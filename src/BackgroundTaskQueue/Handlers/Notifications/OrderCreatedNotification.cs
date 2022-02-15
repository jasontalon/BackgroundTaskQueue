using MediatR;

namespace BackgroundTaskQueue.Handlers.Notifications;

public class CustomerCreatedNotification : INotification
{
    public Guid customerId { get; set; }
    public string? email { get; set; }
}

public class CustomerCreatedNotificationHandler : INotificationHandler<CustomerCreatedNotification>
{
    private readonly ILogger<CustomerCreatedNotificationHandler> _logger;

    public CustomerCreatedNotificationHandler(ILogger<CustomerCreatedNotificationHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CustomerCreatedNotification notification, CancellationToken cancellationToken)
    {
        await Task.Delay(5000, cancellationToken);
        _logger.LogInformation($"Welcome email sent to customer {notification.customerId} email: {notification.email}");
    }
}