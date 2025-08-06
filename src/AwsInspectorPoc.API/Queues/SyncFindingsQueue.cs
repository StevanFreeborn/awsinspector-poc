namespace AwsInspectorPoc.API.Queues;

internal interface ISyncFindingsQueue
{
  ValueTask EnqueueAsync(SyncFindingsQueueItem item, CancellationToken cancellationToken = default);
  IAsyncEnumerable<SyncFindingsQueueItem> DequeueAllAsync(CancellationToken cancellationToken = default);
}

internal sealed class SyncFindingsQueue : ISyncFindingsQueue
{
  private readonly Channel<SyncFindingsQueueItem> _channel = Channel.CreateUnbounded<SyncFindingsQueueItem>();

  public async ValueTask EnqueueAsync(SyncFindingsQueueItem item, CancellationToken cancellationToken = default)
  {
    await _channel.Writer.WriteAsync(item, cancellationToken);
  }

  public IAsyncEnumerable<SyncFindingsQueueItem> DequeueAllAsync(CancellationToken cancellationToken = default)
  {
    return _channel.Reader.ReadAllAsync(cancellationToken);
  }
}