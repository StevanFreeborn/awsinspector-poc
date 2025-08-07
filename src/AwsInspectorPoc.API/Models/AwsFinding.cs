namespace AwsInspectorPoc.API.Services;

internal sealed class AwsFinding
{
  public string Arn { get; init; } = string.Empty;
  public string Title { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public DateTime? FirstObservedAt { get; init; }
  public DateTime? LastObservedAt { get; init; }
  public string Severity { get; init; } = string.Empty;
  public string Status { get; init; } = string.Empty;
  public string Type { get; init; } = string.Empty;
  public string ExploitAvailable { get; init; } = string.Empty;
  public string FixAvailable { get; init; } = string.Empty;
  public double? InspectorScore { get; init; }
  public DateTime? UpdatedAt { get; init; }
  public string RemediationRecommendation { get; init; } = string.Empty;
  public string RemediationRecommendationUrl { get; init; } = string.Empty;
}