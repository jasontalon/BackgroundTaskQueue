using System.Threading.Channels;
using MediatR;

namespace BackgroundTaskQueue;

// https://docs.microsoft.com/en-us/dotnet/core/extensions/queue-service

public interface ITaskQueue
{
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken);

    ValueTask EnqueueAsync(INotification notification);
}

public class TaskQueue : ITaskQueue
{
    private readonly IMediator _mediator;
    private readonly ILogger<TaskQueue>? _logger;
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public TaskQueue(IMediator mediator, ILogger<TaskQueue>? logger)
    {
        _queue = Channel.CreateUnbounded<Func<CancellationToken, ValueTask>>();
        _mediator = mediator;
        _logger = logger;
    }

    public async ValueTask EnqueueAsync(
        INotification notification)
    {
        if (notification is null)
            throw new ArgumentNullException(nameof(notification));

        _logger?.LogInformation($"Queue for {notification.GetType().Name}");

        await _queue.Writer.WriteAsync(
            async cancellationToken => await _mediator.Publish(notification, cancellationToken)
        );
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        var workItem =
            await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}