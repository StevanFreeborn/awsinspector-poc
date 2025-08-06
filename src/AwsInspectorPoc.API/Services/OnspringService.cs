namespace AwsInspectorPoc.API.Services;

internal interface IOnspringService
{
  Task AddOrUpdateResourceAsync(AwsResource resource);
}

internal sealed class OnspringService : IOnspringService, IDisposable
{
  private readonly IOptionsMonitor<OnspringOptions> _options;
  private readonly HttpClient _httpClient;
  private readonly OnspringClient _onspringClient;

  public OnspringService(IOptionsMonitor<OnspringOptions> options, IHttpClientFactory httpClientFactory)
  {
    _options = options;
    _httpClient = httpClientFactory.CreateClient();
    _httpClient.BaseAddress = new Uri(_options.CurrentValue.BaseUrl);
    _onspringClient = new OnspringClient(_options.CurrentValue.ApiKey, _httpClient);
  }

  public async Task AddOrUpdateResourceAsync(AwsResource resource)
  {
    var queryRequest = new QueryRecordsRequest()
    {
      AppId = _options.CurrentValue.BusinessApplicationAppId,
      FieldIds = [_options.CurrentValue.BusinessApplicationNameFieldId],
      Filter = $"{_options.CurrentValue.BusinessApplicationNameFieldId} eq '{resource.Arn}'",
    };
    var queryResult = await _onspringClient.QueryRecordsAsync(queryRequest);

    if (queryResult.IsSuccessful is false)
    {
      throw new Exception($"Query for resource {resource.Arn} failed");
    }

    var existingRecordId = queryResult.Value.Items.FirstOrDefault()?.RecordId ?? 0;

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

  public void Dispose()
  {
    _httpClient.Dispose();
  }
}