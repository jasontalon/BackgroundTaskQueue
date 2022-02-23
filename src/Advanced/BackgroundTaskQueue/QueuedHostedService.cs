namespace BackgroundTaskQueue;

public sealed class QueuedHostedService : BackgroundService
{
    private readonly ITaskQueue _taskQueue;
    private readonly ILogger<QueuedHostedService> _logger;

    public QueuedHostedService(
        ITaskQueue taskQueue,
        ILogger<QueuedHostedService> logger) =>
        (_taskQueue, _logger) = (taskQueue, logger);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItems =
                    await _taskQueue.DequeueAsync(3, TimeSpan.FromSeconds(1), stoppingToken);

                if (workItems.Count > 0)
                    await Task.WhenAll(workItems.Select(async work => await work(stoppingToken)));
                else
                    _logger.LogInformation("No work items. Continue");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation Canceled.");
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task work item.");
            }
        }

        _logger.LogInformation("Exiting...");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{nameof(QueuedHostedService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}