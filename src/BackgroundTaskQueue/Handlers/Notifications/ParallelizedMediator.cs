using MediatR;

namespace BackgroundTaskQueue.Handlers.Notifications;

//https://github.com/jbogard/MediatR/tree/49f9821547db1f86f01851baa1c1935b12029d06/samples/MediatR.Examples.PublishStrategies
public class ParallelizedMediator : Mediator
{
    public ParallelizedMediator(ServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    protected override Task PublishCore(IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers,
        INotification notification, CancellationToken cancellationToken)
    {
        return PublishParallelWhenAll(allHandlers, notification, cancellationToken);
    }

    private Task PublishParallelWhenAll(IEnumerable<Func<INotification, CancellationToken, Task>> handlers,
        INotification notification, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var handler in handlers)
        {
            tasks.Add(Task.Run(() => handler(notification, cancellationToken)));
        }

        return Task.WhenAll(tasks);
    }
}