namespace BackgroundTaskQueue.Handlers.Notifications;
public class IndexCustomerNotificationCommandHandler : BaseNotificationHandler<CustomerCreatedNotification>
{
    public IndexCustomerNotificationCommandHandler(ILogger<IndexCustomerNotificationCommandHandler> logger) :
        base(logger)
    {
    }

    protected override async Task HandleCommand(CustomerCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        var rand = new Random();

        await Task.Delay(TimeSpan.FromSeconds(rand.Next(1, 3)), cancellationToken);
    }
}