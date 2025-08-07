namespace AwsInspectorPoc.API.Options;

internal sealed class OnspringOptions
{
  public string BaseUrl { get; init; } = string.Empty;
  public string ApiKey { get; init; } = string.Empty;
  public int BusinessApplicationAppId { get; init; }
  public int BusinessApplicationNameFieldId { get; init; }
  public int BusinessApplicationStatusFieldId { get; init; }
  public Guid BusinessApplicationOperationalStatusListValueId { get; init; }
  public int BusinessApplicationPlatformFieldId { get; init; }
  public Guid BusinessApplicationAwsPlatformListValueId { get; init; }
  public int BusinessApplicationAwsResourceTypeFieldId { get; init; }
  public int BusinessApplicationLastCompletedSyncFieldId { get; init; }
  public int VulnerabilitiesAppId { get; init; }
  public int VulnerabilitiesUniqueIdFieldId { get; init; }
  public int VulnerabilitiesAwsArnFieldId { get; init; }
  public int VulnerabilitiesScannerSourceFieldId { get; init; }
  public Guid VulnerabilitiesScannerSourceAwsInspectorListValueId { get; init; }
  public int VulnerabilitiesAwsSeverityFieldId { get; init; }
  public int VulnerabilitiesNameFieldId { get; init; }
  public int VulnerabilitiesAwsDescriptionFieldId { get; init; }
  public int VulnerabilitiesAwsFirstObservedAtFieldId { get; init; }
  public int VulnerabilitiesAwsLastObservedAtFieldId { get; init; }
  public int VulnerabilitiesAwsStatusFieldId { get; init; }
  public int VulnerabilitiesAwsTypeFieldId { get; init; }
  public int VulnerabilitiesAwsExploitAvailableFieldId { get; init; }
  public int VulnerabilitiesAwsFixAvailableFieldId { get; init; }
  public int VulnerabilitiesAwsInspectorScoreFieldId { get; init; }
  public int VulnerabilitiesAwsUpdatedAtFieldId { get; init; }
  public int VulnerabilitiesAwsRemediationRecommendationFieldId { get; init; }
  public int VulnerabilitiesAwsRemediationRecommendationUrlFieldId { get; init; }
  public int VulnerabilitiesBusinessApplicationsFieldId { get; init; }
}

internal sealed class OnspringOptionsSetup(IConfiguration configuration) : IConfigureOptions<OnspringOptions>
{
  private const string SectionName = nameof(OnspringOptions);
  private readonly IConfiguration _configuration = configuration;

  public void Configure(OnspringOptions options)
  {
    _configuration.GetSection(SectionName).Bind(options);
  }
}