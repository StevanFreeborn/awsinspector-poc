namespace AwsInspectorPoc.API.Options;

internal sealed class SeqOptions
{
  public string ServerUrl { get; set; } = string.Empty;
  public string ApiKeyHeader { get; set; } = string.Empty;
  public string ApiKey { get; set; } = string.Empty;
  public bool IsEnabled => !string.IsNullOrWhiteSpace(ServerUrl) && !string.IsNullOrWhiteSpace(ApiKeyHeader) && !string.IsNullOrWhiteSpace(ApiKey);
  public string LogEndpoint => $"{ServerUrl}/ingest/otlp/v1/logs";
  public string TraceEndpoint => $"{ServerUrl}/ingest/otlp/v1/traces";
  public string AuthHeader => $"{ApiKeyHeader}={ApiKey}";
}

internal sealed class SeqOptionsSetup(IConfiguration configuration) : IConfigureOptions<SeqOptions>
{
  private const string SectionName = nameof(SeqOptions);
  private readonly IConfiguration _configuration = configuration;

  public void Configure(SeqOptions options)
  {
    _configuration.GetSection(SectionName).Bind(options);
  }
}