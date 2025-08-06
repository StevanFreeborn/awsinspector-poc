namespace AwsInspectorPoc.API.Services;

internal interface IAwsResourceService
{
  IAsyncEnumerable<AwsResource> GetResourcesAsync();
}

internal sealed class AwsResourceService : IAwsResourceService, IDisposable
{
  private readonly IOptionsMonitor<AwsOptions> _awsOptions;
  private readonly AmazonResourceExplorer2Client _resourceClient;

  public AwsResourceService(IOptionsMonitor<AwsOptions> awsOptions)
  {
    _awsOptions = awsOptions;

    var credentials = new BasicAWSCredentials(_awsOptions.CurrentValue.AccessKey, _awsOptions.CurrentValue.SecretKey);
    var regionEndpoint = RegionEndpoint.GetBySystemName(_awsOptions.CurrentValue.Region);
    _resourceClient = new AmazonResourceExplorer2Client(credentials, regionEndpoint);
  }

  public async IAsyncEnumerable<AwsResource> GetResourcesAsync()
  {
    var request = new ListResourcesRequest() { ViewArn = _awsOptions.CurrentValue.ViewArn };
    var paginator = _resourceClient.Paginators.ListResources(request);

    await foreach (var resource in paginator.Resources)
    {
      yield return new AwsResource
      {
        Arn = resource.Arn,
        Type = resource.ResourceType,
      };
    }
  }

  public void Dispose()
  {
    _resourceClient?.Dispose();
  }
}