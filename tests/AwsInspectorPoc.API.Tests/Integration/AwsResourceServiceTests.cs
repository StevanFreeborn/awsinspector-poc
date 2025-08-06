namespace AwsInspectorPoc.API.Tests.Integration;

public sealed class AwsResourceServiceTests : IClassFixture<TestConfiguration>
{
  private readonly AwsResourceService _sut;

  public AwsResourceServiceTests(TestConfiguration testConfig)
  {
    var awsOptions = testConfig.Configuration.GetSection(nameof(AwsOptions)).Get<AwsOptions>();

    if (awsOptions is null)
    {
      throw new InvalidOperationException("AWS options are not configured properly.");
    }

    var mockOptions = new Mock<IOptionsMonitor<AwsOptions>>();
    mockOptions.Setup(m => m.CurrentValue).Returns(awsOptions);

    _sut = new AwsResourceService(mockOptions.Object);
  }

  [Fact]
  public async Task GetResourcesAsync_WhenCalled_ItShouldReturnResources()
  {
    var resources = _sut.GetResourcesAsync();
    var count = 0;

    await foreach (var resource in resources)
    {
      Assert.False(string.IsNullOrWhiteSpace(resource.Arn));
      count++;
    }

    Assert.NotEqual(0, count);
  }
}