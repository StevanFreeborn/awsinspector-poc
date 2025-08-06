namespace AwsInspectorPoc.API.Tests.Fixtures;

public class TestConfiguration
{
  public IConfiguration Configuration { get; } = new ConfigurationBuilder()
    .AddJsonFile("appsettings.Development.json", optional: false)
    .AddJsonFile("appsettings.Testing.json", optional: true)
    .Build();

  public string TestResourceArn => Configuration[nameof(TestResourceArn)] ?? throw new InvalidOperationException("TestResourceArn is not configured.");
}