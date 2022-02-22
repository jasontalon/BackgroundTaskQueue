namespace BackgroundTaskQueue.Handlers.Notifications;
public class SendCustomerWelcomeEmailNotificationCommandHandler : BaseNotificationHandler<CustomerCreatedNotification>
{
    public SendCustomerWelcomeEmailNotificationCommandHandler(
        ILogger<SendCustomerWelcomeEmailNotificationCommandHandler> logger) : base(logger)
    {
    }

    protected override async Task HandleCommand(CustomerCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        var rand = new Random();

        await Task.Delay(TimeSpan.FromSeconds(rand.Next(1, 3)), cancellationToken);
    }
}