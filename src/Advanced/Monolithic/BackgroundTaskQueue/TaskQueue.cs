using System.Threading.Channels;
using MediatR;

namespace BackgroundTaskQueue;

// https://docs.microsoft.com/en-us/dotnet/core/extensions/queue-service

public interface ITaskQueue
{
    ValueTask<List<Func<CancellationToken, ValueTask>>> DequeueAsync(int batchSize = 1, TimeSpan? timeOut = null,
        CancellationToken cancellationToken = default);

    ValueTask EnqueueAsync(INotification notification);
}

public class TaskQueue : ITaskQueue
{
    private readonly IMediator _mediator;
    private readonly ILogger<TaskQueue>? _logger;
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public TaskQueue(ParallelizedMediator mediator, ILogger<TaskQueue>? logger)
    {
        _queue = Channel.CreateUnbounded<Func<CancellationToken, ValueTask>>();
        _mediator = mediator;
        _logger = logger;
    }

    //https://stackoverflow.com/questions/63881607/how-to-read-remaining-items-in-channel-less-than-batch-size-if-there-is-no-new
    public async ValueTask<List<Func<CancellationToken, ValueTask>>> DequeueAsync(int batchSize = 1,
        TimeSpan? timeOut = null,
        CancellationToken cancellationToken = default)
    {
        if (batchSize == 1 && !timeOut.HasValue)
            return new List<Func<CancellationToken, ValueTask>> {await _queue.Reader.ReadAsync(cancellationToken)};

        timeOut ??= TimeSpan.FromSeconds(1);

        var workItems = new List<Func<CancellationToken, ValueTask>>();

        using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCancellationTokenSource.CancelAfter(timeOut.Value);

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

    public async ValueTask EnqueueAsync(
        INotification notification)
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        _logger?.LogInformation($"Queue for {notification.GetType().Name}");

        await _queue.Writer.WriteAsync(
            async cancellationToken =>
            {
                var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));

                await _mediator.Publish(notification, cancellationTokenSource.Token);
            }
        );
    }
}