namespace AwsInspectorPoc.API.Options;

internal sealed class BasicAuthOptions
{
  public string Username { get; init; } = "admin";
  public string Password { get; init; } = "password";
}

internal sealed class BasicAuthOptionsSetup(IConfiguration configuration) : IConfigureOptions<BasicAuthOptions>
{
  private const string SectionName = nameof(BasicAuthOptions);
  private readonly IConfiguration _configuration = configuration;

  public void Configure(BasicAuthOptions options)
  {
    _configuration.GetSection(SectionName).Bind(options);
  }
}