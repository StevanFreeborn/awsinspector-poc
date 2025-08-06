using AwsInspectorPoc.API.Models;

namespace AwsInspectorPoc.API.Tests.Integration;

public sealed class OnspringServiceTests : IClassFixture<TestConfiguration>, IDisposable
{
  private readonly HttpClient _httpClient = new();
  private readonly OnspringService _sut;

  public OnspringServiceTests(TestConfiguration testConfig)
  {
    var onspringOptions = testConfig.Configuration.GetSection(nameof(OnspringOptions)).Get<OnspringOptions>();

    if (onspringOptions is null)
    {
      throw new InvalidOperationException("Onspring options are not configured properly.");
    }

    var mockOptions = new Mock<IOptionsMonitor<OnspringOptions>>();
    mockOptions.Setup(m => m.CurrentValue).Returns(onspringOptions);

    var mockClientFactory = new Mock<IHttpClientFactory>();
    mockClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);

    _sut = new OnspringService(mockOptions.Object, mockClientFactory.Object);
  }

  [Fact]
  public async Task AddOrUpdateResourceAsync_WhenCalled_ItShouldAddOrUpdateResource()
  {
    var resource = new AwsResource
    {
      Arn = Guid.NewGuid().ToString(),
      Type = Guid.NewGuid().ToString(),
    };

    await _sut.AddOrUpdateResourceAsync(resource);
  }

  public void Dispose()
  {
    _httpClient.Dispose();
  }
}