using System.Diagnostics;
using MediatR;

namespace BackgroundTaskQueue.Handlers.Notifications.Tasks;

public abstract class BaseTaskHandler<T> : INotificationHandler<T>
    where T : INotification
{
    public readonly ILogger Logger;

    public BaseTaskHandler(ILogger logger)
    {
        Logger = logger;
    }

    public async Task Handle(T notification, CancellationToken cancellationToken)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        await HandleCommand(notification, cancellationToken);
        stopWatch.Stop();
        Logger.LogInformation($"{GetType().Name} Finished. {stopWatch.Elapsed.Seconds}s");
    }

    protected abstract Task HandleCommand(T notification, CancellationToken cancellationToken);
}