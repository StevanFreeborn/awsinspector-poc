namespace AwsInspectorPoc.API.Queues;

internal sealed class SyncFindingsQueueProcessor(
  ISyncFindingsQueue queue,
  IAwsInspectorService awsInspectorService,
  IOnspringService onspringService,
  ILogger<SyncFindingsQueueProcessor> logger
) : BackgroundService
{
  private readonly ISyncFindingsQueue _queue = queue;
  private readonly IAwsInspectorService _awsInspectorService = awsInspectorService;
  private readonly IOnspringService _onspringService = onspringService;
  private readonly ILogger<SyncFindingsQueueProcessor> _logger = logger;

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
    await Parallel.ForEachAsync(
      _awsInspectorService.GetFindingsForResourceAsync(item.ResourceArn),
      cancellationToken,
      async (finding, _) => await _onspringService.AddOrUpdateFindingAsync(item.ResourceArn, finding)
    );

    await _onspringService.UpdateResourceLastCompletedSyncAsync(item.ResourceArn);
  }
}