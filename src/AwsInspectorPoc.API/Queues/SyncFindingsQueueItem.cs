namespace AwsInspectorPoc.API.Queues;

internal sealed record SyncFindingsQueueItem
{
  public string ResourceArn { get; init; } = string.Empty;

  private SyncFindingsQueueItem(string resourceArn)
  {
    ResourceArn = resourceArn;
  }

  public static SyncFindingsQueueItem From(SyncFindingsRequest request)
  {
    return new SyncFindingsQueueItem(request.ResourceArn);
  }
}