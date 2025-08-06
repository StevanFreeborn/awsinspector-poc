namespace AwsInspectorPoc.API.Services;

internal interface IAwsResourceService
{
  IAsyncEnumerable<AwsResource> GetResourcesAsync();
}

internal sealed class AwsResourceService : IAwsResourceService
{
  private readonly IOptionsMonitor<AwsOptions> _awsOptions;
  private readonly BasicAWSCredentials _credentials;

  public AwsResourceService(IOptionsMonitor<AwsOptions> awsOptions)
  {
    _awsOptions = awsOptions;

    _credentials = new BasicAWSCredentials(_awsOptions.CurrentValue.AccessKey, _awsOptions.CurrentValue.SecretKey);
  }

  public async IAsyncEnumerable<AwsResource> GetResourcesAsync()
  {
    foreach (var region in _awsOptions.CurrentValue.Regions)
    {
      var regionEndpoint = RegionEndpoint.GetBySystemName(region);
      using var resourceClient = new AmazonResourceExplorer2Client(_credentials, regionEndpoint);
      var request = new ListResourcesRequest();
      var paginator = resourceClient.Paginators.ListResources(request);

      await foreach (var resource in paginator.Resources)
      {
        yield return new AwsResource
        {
          Arn = resource.Arn,
          Type = resource.ResourceType,
        };
      }
    }
  }
}