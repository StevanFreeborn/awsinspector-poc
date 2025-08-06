using Amazon.Inspector2;
using Amazon.Inspector2.Model;

using StringComparison = Amazon.Inspector2.StringComparison;

namespace AwsInspectorPoc.API.Services;

internal interface IAwsInspectorService
{
  IAsyncEnumerable<AwsFinding> GetFindingsForResourceAsync(string resourceArn);
}

internal sealed class AwsInspectorService : IAwsInspectorService
{
  private readonly IOptionsMonitor<AwsOptions> _options;
  private readonly BasicAWSCredentials _credentials;

  public AwsInspectorService(IOptionsMonitor<AwsOptions> options)
  {
    _options = options;
    _credentials = new BasicAWSCredentials(_options.CurrentValue.AccessKey, _options.CurrentValue.SecretKey);
  }

  public async IAsyncEnumerable<AwsFinding> GetFindingsForResourceAsync(string resourceArn)
  {
    foreach (var region in _options.CurrentValue.Regions)
    {
      var regionEndpoint = RegionEndpoint.GetBySystemName(region);
      using var inspectorClient = new AmazonInspector2Client(_credentials, regionEndpoint);
      var request = new ListFindingsRequest
      {
        FilterCriteria = new()
        {
          ResourceId = [
            new()
            {
              Comparison = StringComparison.EQUALS,
              Value = resourceArn.Split('/').Last()
            }
          ]
        }
      };
      var paginator = inspectorClient.Paginators.ListFindings(request);

      await foreach (var finding in paginator.Findings)
      {
        yield return new AwsFinding
        {
          Description = finding.Description
        };
      }
    }
  }
}