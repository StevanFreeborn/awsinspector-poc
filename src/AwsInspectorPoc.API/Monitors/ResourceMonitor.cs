namespace AwsInspectorPoc.API.Monitors;

internal sealed class ResourceMonitor(
  IOptionsMonitor<ResourceMonitorOptions> options,
  ILogger<ResourceMonitor> logger,
  TimeProvider timeProvider,
  IAwsResourceService awsResourceService,
  IOnspringService onspringService
) : IHostedService, IDisposable
{
  private ITimer? _timer;
  private readonly TimeProvider _timeProvider = timeProvider;
  private readonly ILogger<ResourceMonitor> _logger = logger;
  private readonly IOptionsMonitor<ResourceMonitorOptions> _options = options;
  private readonly IAwsResourceService _awsResourceService = awsResourceService;
  private readonly IOnspringService _onspringService = onspringService;

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Starting Resource Monitor...");

    _timer = _timeProvider.CreateTimer(
      async _ => await CreateOrUpdateResourcesAsync(),
      null,
      TimeSpan.Zero,
      _options.CurrentValue.PollingInterval
    );

    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Stopping Resource Monitor...");

    _timer?.Change(Timeout.InfiniteTimeSpan, TimeSpan.Zero);

    return Task.CompletedTask;
  }

  public void Dispose()
  {
    _timer?.Dispose();
  }

  private async Task CreateOrUpdateResourcesAsync()
  {
    _logger.LogInformation("Processing resources...");
    var startTimeStamp = _timeProvider.GetTimestamp();

    await Parallel.ForEachAsync(
      _awsResourceService.GetResourcesAsync(),
      async (resource, _) =>
      {
        try
        {
          await _onspringService.AddOrUpdateResourceAsync(resource);
          _logger.LogInformation("Successfully processed resource: {ResourceArn}", resource.Arn);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Failed to process resource: {ResourceArn}", resource.Arn);
        }
      }
    );

    var elapsedTime = _timeProvider.GetElapsedTime(startTimeStamp);
    _logger.LogInformation("Finished processing resources in {ElapsedTime}", elapsedTime.ToString("c"));
  }
}