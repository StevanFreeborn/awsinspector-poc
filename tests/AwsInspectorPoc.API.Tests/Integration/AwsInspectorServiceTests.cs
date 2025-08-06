namespace AwsInspectorPoc.API.Tests.Integration;

public class AwsInspectorServiceTests : IClassFixture<TestConfiguration>
{
  private readonly string _testResourceArn;
  private readonly AwsInspectorService _sut;

  public AwsInspectorServiceTests(TestConfiguration testConfig)
  {
    _testResourceArn = testConfig.TestResourceArn;

    var awsOptions = testConfig.Configuration.GetSection(nameof(AwsOptions)).Get<AwsOptions>();

    if (awsOptions is null)
    {
      throw new InvalidOperationException("AWS options are not configured properly.");
    }

    var mockOptions = new Mock<IOptionsMonitor<AwsOptions>>();
    mockOptions.Setup(m => m.CurrentValue).Returns(awsOptions);

    _sut = new AwsInspectorService(mockOptions.Object);
  }

  [Fact]
  public async Task GetFindingsForResourceAsync_WhenCalled_ItShouldReturnFindings()
  {
    var findings = _sut.GetFindingsForResourceAsync(_testResourceArn);
    var count = 0;

    await foreach (var finding in findings)
    {
      Assert.False(string.IsNullOrWhiteSpace(finding.Description));
      count++;
    }

    Assert.NotEqual(0, count);
  }
}