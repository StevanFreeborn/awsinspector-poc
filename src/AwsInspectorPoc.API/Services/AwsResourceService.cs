namespace AwsInspectorPoc.API.Services;

internal sealed class AwsResourceService : IDisposable
{
  private readonly AwsOptions _awsOptions;
  private readonly AmazonResourceExplorer2Client _resourceClient;

  public AwsResourceService(AwsOptions awsOptions)
  {
    _awsOptions = awsOptions;

    var credentials = new BasicAWSCredentials(_awsOptions.AccessKey, _awsOptions.SecretKey);
    var regionEndpoint = RegionEndpoint.GetBySystemName(_awsOptions.Region);
    _resourceClient = new AmazonResourceExplorer2Client(credentials, regionEndpoint);
  }

  public async IAsyncEnumerable<string> GetResourcesAsync()
  {
    var request = new ListResourcesRequest() { ViewArn = _awsOptions.ViewArn };
    var paginator = _resourceClient.Paginators.ListResources(request);

    await foreach (var resource in paginator.Resources)
    {
      yield return resource.Arn;
    }
  }

  public void Dispose()
  {
    _resourceClient?.Dispose();
  }
}