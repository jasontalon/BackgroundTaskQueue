namespace BackgroundTaskQueue.Handlers.Notifications;
public class SendPromotionsNotificationCommandHandler : BaseNotificationHandler<CustomerCreatedNotification>
{
    public SendPromotionsNotificationCommandHandler(ILogger<SendPromotionsNotificationCommandHandler> logger) :
        base(logger)
    {
    }

    protected override async Task HandleCommand(CustomerCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        var rand = new Random();

        await Task.Delay(TimeSpan.FromSeconds(6), cancellationToken);
    }
}