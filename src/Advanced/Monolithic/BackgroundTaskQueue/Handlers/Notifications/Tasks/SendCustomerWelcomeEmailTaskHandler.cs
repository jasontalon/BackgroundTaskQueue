namespace BackgroundTaskQueue.Handlers.Notifications.Tasks;

public class SendCustomerWelcomeEmailTaskHandler : BaseTaskHandler<CustomerCreatedNotification>
{
    public SendCustomerWelcomeEmailTaskHandler(
        ILogger<SendCustomerWelcomeEmailTaskHandler> logger) : base(logger)
    {
    }

    protected override async Task HandleCommand(CustomerCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 3)), cancellationToken);
    }
}