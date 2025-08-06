namespace AwsInspectorPoc.API.Queues;

internal sealed class SyncFindingsQueueProcessor(
  ISyncFindingsQueue queue,
  IAwsInspectorService awsInspectorService,
  IOnspringService onspringService
) : BackgroundService
{
  private readonly ISyncFindingsQueue _queue = queue;
  private readonly IAwsInspectorService _awsInspectorService = awsInspectorService;
  private readonly IOnspringService _onspringService = onspringService;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (stoppingToken.IsCancellationRequested is false)
    {
      await foreach (var item in _queue.DequeueAllAsync(stoppingToken))
      {
        await ProcessQueueItemAsync(item, stoppingToken);
      }
    }
  }

  private async Task ProcessQueueItemAsync(SyncFindingsQueueItem item, CancellationToken cancellationToken)
  {
    await Parallel.ForEachAsync(
      _awsInspectorService.GetFindingsForResourceAsync(item.ResourceArn),
      cancellationToken,
      async (finding, _) => await _onspringService.AddOrUpdateFindingAsync(item.OnspringRecordId, finding));
  }
}