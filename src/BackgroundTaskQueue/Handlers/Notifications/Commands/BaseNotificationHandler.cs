using System.Diagnostics;
using MediatR;

namespace BackgroundTaskQueue.Handlers.Notifications;

public abstract class BaseNotificationHandler<T> : INotificationHandler<T>
    where T : INotification
{
    public readonly ILogger Logger;

    public BaseNotificationHandler(ILogger logger)
    {
        Logger = logger;
    }

    public async Task Handle(T notification, CancellationToken cancellationToken)
    {
        var name = GetType().Name;
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        await HandleCommand(notification, cancellationToken);
        stopWatch.Stop();
        Logger.LogInformation($"{name} Finished. {stopWatch.Elapsed.Seconds}s");
    }

    protected abstract Task HandleCommand(T notification, CancellationToken cancellationToken);
}