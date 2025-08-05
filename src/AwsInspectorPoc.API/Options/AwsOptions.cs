namespace AwsInspectorPoc.API.Options;

internal sealed class AwsOptions
{
  public string AccessKey { get; init; } = string.Empty;
  public string SecretKey { get; init; } = string.Empty;
  public string Region { get; init; } = string.Empty;
  public string ViewArn { get; init; } = string.Empty;
}