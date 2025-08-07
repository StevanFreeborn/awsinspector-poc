namespace AwsInspectorPoc.API.Queues;

internal sealed class SyncFindingsQueueProcessor(
  ISyncFindingsQueue queue,
  IAwsInspectorService awsInspectorService,
  IOnspringService onspringService,
  ILogger<SyncFindingsQueueProcessor> logger,
  TimeProvider timeProvider
) : BackgroundService
{
  private readonly ISyncFindingsQueue _queue = queue;
  private readonly IAwsInspectorService _awsInspectorService = awsInspectorService;
  private readonly IOnspringService _onspringService = onspringService;
  private readonly ILogger<SyncFindingsQueueProcessor> _logger = logger;
  private readonly TimeProvider _timeProvider = timeProvider;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (stoppingToken.IsCancellationRequested is false)
    {
      await foreach (var item in _queue.DequeueAllAsync(stoppingToken))
      {
        try
        {
          await ProcessQueueItemAsync(item, stoppingToken);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error processing queue item for resource {ResourceArn}", item.ResourceArn);
        }
      }
    }
  }

  private async Task ProcessQueueItemAsync(SyncFindingsQueueItem item, CancellationToken cancellationToken)
  {
    _logger.LogInformation("Processing findings for resource {ResourceArn}", item.ResourceArn);
    var startTimeStamp = _timeProvider.GetTimestamp();
    var count = 0;

    await Parallel.ForEachAsync(
      _awsInspectorService.GetFindingsForResourceAsync(item.ResourceArn),
      cancellationToken,
      async (finding, _) =>
      {
        Interlocked.Increment(ref count);

        try
        {
          await _onspringService.AddOrUpdateFindingAsync(item.ResourceArn, finding);
          _logger.LogInformation("Successfully processed finding {FindingId} for resource {ResourceArn}", finding.Arn, item.ResourceArn);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error adding or updating finding {FindingId} for resource {ResourceArn}", finding.Arn, item.ResourceArn);
        }
      }
    );

    await _onspringService.UpdateResourceLastCompletedSyncAsync(item.ResourceArn);
    var elapsedTime = _timeProvider.GetElapsedTime(startTimeStamp);
    _logger.LogInformation("Finished processing {Count} findings for resource {ResourceArn} in {ElapsedTime} ms", count, item.ResourceArn, elapsedTime.TotalMilliseconds);
  }
}