namespace AwsInspectorPoc.API.Options;

internal sealed class ResourceMonitorOptions
{
  public bool Enabled { get; init; }
  public TimeSpan PollingInterval { get; init; } = TimeSpan.FromHours(1);
}

internal sealed class ResourceMonitorOptionsSetup(IConfiguration configuration) : IConfigureOptions<ResourceMonitorOptions>
{
  private const string SectionName = nameof(ResourceMonitorOptions);
  private readonly IConfiguration _configuration = configuration;

  public void Configure(ResourceMonitorOptions options)
  {
    _configuration.GetSection(SectionName).Bind(options);
  }
}