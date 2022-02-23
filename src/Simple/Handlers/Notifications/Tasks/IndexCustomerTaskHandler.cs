namespace BackgroundTaskQueue.Handlers.Notifications.Tasks;

public class IndexCustomerTaskHandler : BaseTaskHandler<CustomerCreatedNotification>
{
    public IndexCustomerTaskHandler(ILogger<IndexCustomerTaskHandler> logger) :
        base(logger)
    {
    }

    protected override async Task HandleCommand(CustomerCreatedNotification notification,
        CancellationToken cancellationToken)
    {    
        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 3)), cancellationToken);
    }
}