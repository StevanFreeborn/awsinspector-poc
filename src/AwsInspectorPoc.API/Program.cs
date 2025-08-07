var builder = WebApplication.CreateBuilder(args);

builder.AddTelemetry();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpClient();

builder.Services.ConfigureOptions<AwsOptionsSetup>();
builder.Services.AddSingleton<IAwsResourceService, AwsResourceService>();
builder.Services.AddSingleton<IAwsInspectorService, AwsInspectorService>();

builder.Services.ConfigureOptions<OnspringOptionsSetup>();
builder.Services.AddSingleton<IOnspringService, OnspringService>();

builder.Services.ConfigureOptions<ResourceMonitorOptionsSetup>();
builder.Services.AddHostedService<ResourceMonitor>();

builder.Services.AddSingleton<ISyncFindingsQueue, SyncFindingsQueue>();
builder.Services.AddHostedService<SyncFindingsQueueProcessor>();

builder.Services.AddOpenApi();

builder.Services.ConfigureOptions<BasicAuthOptionsSetup>();
builder.Services.AddAuthentication()
  .AddScheme<AuthenticationSchemeOptions, BasicAuthentication>(
    BasicAuthentication.SchemeName,
    null
  );

builder.Services.AddAuthorizationBuilder()
  .AddPolicy(BasicAuthentication.SchemeName, policy =>
  {
    policy.AuthenticationSchemes.Add(BasicAuthentication.SchemeName);
    policy.RequireAuthenticatedUser();
    policy.RequireClaim(ClaimTypes.NameIdentifier);
  });

var app = builder.Build();

app
  .MapGet("/", () => "I'm alive, I'm alive, I'm alive 🎵")
  .AllowAnonymous();

app
  .MapPost("/sync-findings", async (
    [FromBody] SyncFindingsRequest request,
    [FromServices] ISyncFindingsQueue queue,
    [FromServices] TimeProvider timeProvider
  ) =>
  {
    if (request.IsValid() is false)
    {
      return Results.BadRequest($"Invalid request: {nameof(request.ResourceArn)} should not be empty.");
    }

    await queue.EnqueueAsync(SyncFindingsQueueItem.From(request));

    return Results.Ok(new { DateSyncRequested = timeProvider.GetUtcNow() });
  })
  .RequireAuthorization(BasicAuthentication.SchemeName);

if (app.Environment.IsProduction())
{
  app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();

record SyncFindingsRequest(string ResourceArn)
{
  public bool IsValid()
  {
    return string.IsNullOrWhiteSpace(ResourceArn) is false;
  }
}