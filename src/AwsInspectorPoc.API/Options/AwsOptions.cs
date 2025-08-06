namespace AwsInspectorPoc.API.Options;

internal sealed class AwsOptions
{
  public string AccessKey { get; init; } = string.Empty;
  public string SecretKey { get; init; } = string.Empty;
  public List<string> Regions { get; init; } = [];
}

internal sealed class AwsOptionsSetup(IConfiguration configuration) : IConfigureOptions<AwsOptions>
{
  private const string SectionName = nameof(AwsOptions);
  private readonly IConfiguration _configuration = configuration;

  public void Configure(AwsOptions options)
  {
    _configuration.GetSection(SectionName).Bind(options);
  }
}