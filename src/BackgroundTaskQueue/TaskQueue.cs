using System.Threading.Channels;
using BackgroundTaskQueue.Handlers.Notifications;
using MediatR;

namespace BackgroundTaskQueue;

// https://docs.microsoft.com/en-us/dotnet/core/extensions/queue-service

public interface ITaskQueue
{
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken);

    ValueTask<List<Func<CancellationToken, ValueTask>>> BatchDequeueAsync(int batchSize, TimeSpan timeOut,
        CancellationToken cancellationToken = default);

    ValueTask QueueNotification(INotification notification);
}

public class TaskQueue : ITaskQueue
{
    private readonly IMediator _mediator;
    private readonly ILogger<TaskQueue>? _logger;
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public TaskQueue(int capacity, ParallelizedMediator mediator, ILogger<TaskQueue>? logger)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
        _mediator = mediator;
        _logger = logger;
    }

    //https://stackoverflow.com/questions/63881607/how-to-read-remaining-items-in-channel-less-than-batch-size-if-there-is-no-new
    public async ValueTask<List<Func<CancellationToken, ValueTask>>> BatchDequeueAsync(int batchSize,
        TimeSpan timeOut,
        CancellationToken cancellationToken = default)
    {
        var workItems = new List<Func<CancellationToken, ValueTask>>();

        using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCancellationTokenSource.CancelAfter(timeOut);

        while (true)
        {
            var _cancellationToken = workItems.Count == 0 ? cancellationToken : linkedCancellationTokenSource.Token;
            Func<CancellationToken, ValueTask> workItem;

            try
            {
                workItem = await _queue.Reader.ReadAsync(_cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                cancellationToken.ThrowIfCancellationRequested();
                break; // The cancellation was induced by timeout (ignore it)
            }
            catch (ChannelClosedException)
            {
                if (workItems.Count == 0) throw;
                break;
            }

            workItems.Add(workItem);
            if (workItems.Count >= batchSize) break;
        }

        return workItems;
    }

    public async ValueTask QueueNotification(
        INotification notification)
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        _logger?.LogInformation($"Queue for {notification.GetType().Name}");
        await _queue.Writer.WriteAsync(
            async cancellationToken => { await _mediator.Publish(notification, cancellationToken); }
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