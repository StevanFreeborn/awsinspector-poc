namespace AwsInspectorPoc.API.Queues;

internal sealed record SyncFindingsQueueItem
{
  public string ResourceArn { get; init; } = string.Empty;
  public int OnspringRecordId { get; init; }

  private SyncFindingsQueueItem(string resourceArn, int onspringRecordId)
  {
    ResourceArn = resourceArn;
    OnspringRecordId = onspringRecordId;
  }

  public static SyncFindingsQueueItem From(SyncFindingsRequest request)
  {
    return new SyncFindingsQueueItem(request.ResourceArn, request.OnspringResourceRecordId);
  }
}