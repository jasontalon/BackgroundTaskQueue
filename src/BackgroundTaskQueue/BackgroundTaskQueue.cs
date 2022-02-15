using System.Threading.Channels;
using MediatR;

namespace BackgroundTaskQueue;

// https://docs.microsoft.com/en-us/dotnet/core/extensions/queue-service

public interface ITaskQueue
{
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken);

    ValueTask QueueNotification(INotification notification);
}

public class TaskQueue : ITaskQueue
{
    private readonly IMediator _mediator;
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public TaskQueue(int capacity, IMediator mediator)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
        _mediator = mediator;
    }

    public async ValueTask QueueNotification(
        INotification notification)
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        await _queue.Writer.WriteAsync(
            async (cancellationToken) => { await _mediator.Publish(notification, cancellationToken); }
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