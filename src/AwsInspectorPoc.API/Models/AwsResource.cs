namespace AwsInspectorPoc.API.Models;

internal sealed class AwsResource
{
  public string Arn { get; init; } = string.Empty;
  public string Type { get; init; } = string.Empty;
}