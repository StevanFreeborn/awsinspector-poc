namespace AwsInspectorPoc.API.Tests.Fixtures;

public class TestConfiguration
{
  public IConfiguration Configuration { get; } = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json", optional: false)
    .Build();
}