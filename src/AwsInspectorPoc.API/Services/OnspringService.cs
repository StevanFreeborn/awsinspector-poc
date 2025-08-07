using System.Globalization;

namespace AwsInspectorPoc.API.Services;

internal interface IOnspringService
{
  Task AddOrUpdateFindingAsync(string resourceArn, AwsFinding finding);
  Task AddOrUpdateResourceAsync(AwsResource resource);
  Task UpdateResourceLastCompletedSyncAsync(string resourceArn);
}

internal sealed class OnspringService : IOnspringService, IDisposable
{
  private readonly IOptionsMonitor<OnspringOptions> _options;
  private readonly HttpClient _httpClient;
  private readonly OnspringClient _onspringClient;
  private readonly TimeProvider _timeProvider;

  public OnspringService(
    IOptionsMonitor<OnspringOptions> options,
    IHttpClientFactory httpClientFactory,
    TimeProvider timeProvider
  )
  {
    _options = options;
    _timeProvider = timeProvider;
    _httpClient = httpClientFactory.CreateClient();
    _httpClient.BaseAddress = new Uri(_options.CurrentValue.BaseUrl);
    _onspringClient = new OnspringClient(_options.CurrentValue.ApiKey, _httpClient);
  }

  public async Task AddOrUpdateFindingAsync(string resourceArn, AwsFinding finding)
  {
    var queryRequest = new QueryRecordsRequest()
    {
      AppId = _options.CurrentValue.VulnerabilitiesAppId,
      FieldIds = [
        _options.CurrentValue.VulnerabilitiesAwsArnFieldId,
      ],
      Filter = $"{_options.CurrentValue.VulnerabilitiesAwsArnFieldId} eq '{finding.Arn}'"
    };

    var queryResult = await _onspringClient.QueryRecordsAsync(queryRequest);

    if (queryResult.IsSuccessful is false)
    {
      throw new Exception($"Query for finding {finding.Arn} failed");
    }

    var existingVulnerabilityRecordId = queryResult.Value.Items.FirstOrDefault()?.RecordId ?? 0;
    var onspringResourceRecordId = await GetResourceRecordIdAsync(resourceArn);
    var relatedBusinessApplications = queryResult.Value.Items
      .SelectMany(
        item => item.FieldData
          .FirstOrDefault(fd => fd.FieldId == _options.CurrentValue.VulnerabilitiesBusinessApplicationsFieldId)?
          .AsIntegerList() ?? []
      )
      .ToList();
    relatedBusinessApplications.Add(onspringResourceRecordId);

    var onspringRecord = new ResultRecord()
    {
      AppId = _options.CurrentValue.VulnerabilitiesAppId,
      RecordId = existingVulnerabilityRecordId,
      FieldData = [
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesAwsArnFieldId, finding.Arn),
        new GuidFieldValue(_options.CurrentValue.VulnerabilitiesScannerSourceFieldId, _options.CurrentValue.VulnerabilitiesScannerSourceAwsInspectorListValueId),
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesAwsSeverityFieldId, finding.Severity),
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesNameFieldId, finding.Title),
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesAwsDescriptionFieldId, finding.Description),
        new DateFieldValue(_options.CurrentValue.VulnerabilitiesAwsFirstObservedAtFieldId, finding.FirstObservedAt),
        new DateFieldValue(_options.CurrentValue.VulnerabilitiesAwsLastObservedAtFieldId, finding.LastObservedAt),
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesAwsStatusFieldId, finding.Status),
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesAwsTypeFieldId, finding.Type),
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesAwsExploitAvailableFieldId, finding.ExploitAvailable),
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesAwsFixAvailableFieldId, finding.FixAvailable),
        new DecimalFieldValue(_options.CurrentValue.VulnerabilitiesAwsInspectorScoreFieldId, Convert.ToDecimal(finding.InspectorScore, CultureInfo.InvariantCulture)),
        new DateFieldValue(_options.CurrentValue.VulnerabilitiesAwsUpdatedAtFieldId, finding.UpdatedAt),
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesAwsRemediationRecommendationFieldId, finding.RemediationRecommendation),
        new StringFieldValue(_options.CurrentValue.VulnerabilitiesAwsRemediationRecommendationUrlFieldId, finding.RemediationRecommendationUrl),
        new IntegerListFieldValue(_options.CurrentValue.VulnerabilitiesBusinessApplicationsFieldId, [.. relatedBusinessApplications.Distinct()])
      ]
    };

    var saveResult = await _onspringClient.SaveRecordAsync(onspringRecord);

    if (saveResult.IsSuccessful is false)
    {
      throw new Exception($"Failed to save finding {finding.Arn} to Onspring");
    }
  }

  public async Task AddOrUpdateResourceAsync(AwsResource resource)
  {
    var existingRecordId = await GetResourceRecordIdAsync(resource.Arn);
    var onspringRecord = new ResultRecord()
    {
      AppId = _options.CurrentValue.BusinessApplicationAppId,
      RecordId = existingRecordId,
      FieldData = [
        new StringFieldValue(_options.CurrentValue.BusinessApplicationNameFieldId, resource.Arn),
        new GuidFieldValue(_options.CurrentValue.BusinessApplicationStatusFieldId, _options.CurrentValue.BusinessApplicationOperationalStatusListValueId),
        new GuidFieldValue(_options.CurrentValue.BusinessApplicationPlatformFieldId, _options.CurrentValue.BusinessApplicationAwsPlatformListValueId),
        new StringFieldValue(_options.CurrentValue.BusinessApplicationAwsResourceTypeFieldId, resource.Type),
      ]
    };

    var saveResult = await _onspringClient.SaveRecordAsync(onspringRecord);

    if (saveResult.IsSuccessful is false)
    {
      throw new Exception($"Failed to save resource {resource.Arn} to Onspring");
    }
  }

  private async Task<int> GetResourceRecordIdAsync(string resourceArn)
  {
    var queryRequest = new QueryRecordsRequest()
    {
      AppId = _options.CurrentValue.BusinessApplicationAppId,
      FieldIds = [_options.CurrentValue.BusinessApplicationNameFieldId],
      Filter = $"{_options.CurrentValue.BusinessApplicationNameFieldId} eq '{resourceArn}'",
    };
    var queryResult = await _onspringClient.QueryRecordsAsync(queryRequest);

    if (queryResult.IsSuccessful is false)
    {
      throw new Exception($"Query for resource {resourceArn} failed");
    }

    return queryResult.Value.Items.FirstOrDefault()?.RecordId ?? 0;
  }

  public void Dispose()
  {
    _httpClient.Dispose();
  }

  public async Task UpdateResourceLastCompletedSyncAsync(string resourceArn)
  {
    var resourceRecordId = await GetResourceRecordIdAsync(resourceArn);
    var onspringRecord = new ResultRecord()
    {
      AppId = _options.CurrentValue.BusinessApplicationAppId,
      RecordId = resourceRecordId,
      FieldData = [
        new DateFieldValue(_options.CurrentValue.BusinessApplicationLastCompletedSyncFieldId, _timeProvider.GetUtcNow().DateTime)
      ]
    };

    var saveResult = await _onspringClient.SaveRecordAsync(onspringRecord);

    if (saveResult.IsSuccessful is false)
    {
      throw new Exception($"Failed to update last completed sync for resource {resourceArn}");
    }
  }
}