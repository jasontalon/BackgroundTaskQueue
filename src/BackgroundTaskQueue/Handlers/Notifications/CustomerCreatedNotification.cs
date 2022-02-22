using MediatR;

namespace BackgroundTaskQueue.Handlers.Notifications;

public class CustomerCreatedNotification : INotification
{
    public Guid CustomerId { get; set; }

    public CustomerCreatedNotification()
    {
    }

    public CustomerCreatedNotification(Guid customerId)
    {
        CustomerId = customerId;
    }
}