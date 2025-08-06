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