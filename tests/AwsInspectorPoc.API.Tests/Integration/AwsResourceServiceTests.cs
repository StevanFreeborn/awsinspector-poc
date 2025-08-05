namespace AwsInspectorPoc.API.Tests.Integration;

public class AwsResourceServiceTests : IClassFixture<TestConfiguration>
{
  private readonly AwsResourceService _sut;

  public AwsResourceServiceTests(TestConfiguration testConfig)
  {
    var awsOptions = testConfig.Configuration.GetSection(nameof(AwsOptions)).Get<AwsOptions>();

    if (awsOptions is null)
    {
      throw new InvalidOperationException("AWS options are not configured properly.");
    }

    _sut = new AwsResourceService(awsOptions);
  }

  [Fact]
  public async Task GetResourcesAsync_WhenCalled_ItShouldReturnResources()
  {
    var resources = _sut.GetResourcesAsync();
    var count = 0;

    await foreach (var resource in resources)
    {
      Assert.False(string.IsNullOrWhiteSpace(resource));
      count++;
    }

    Assert.NotEqual(0, count);
  }
}