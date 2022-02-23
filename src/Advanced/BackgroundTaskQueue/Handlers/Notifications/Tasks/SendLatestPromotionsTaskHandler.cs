namespace BackgroundTaskQueue.Handlers.Notifications.Tasks;

public class SendLatestPromotionsTaskHandler : BaseTaskHandler<CustomerCreatedNotification>
{
    public SendLatestPromotionsTaskHandler(ILogger<SendLatestPromotionsTaskHandler> logger) :
        base(logger)
    {
    }

    protected override async Task HandleCommand(CustomerCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(6), cancellationToken);
    }
}