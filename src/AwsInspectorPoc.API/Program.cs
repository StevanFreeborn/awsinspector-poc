// TODO: Implement endpoint that accepts
// a resource ID and adds/or updates all
// findings associated with that resource
// in the Vulnerabilities app in Onspring

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpClient();

builder.Services.ConfigureOptions<AwsOptionsSetup>();
builder.Services.AddSingleton<IAwsResourceService, AwsResourceService>();

builder.Services.ConfigureOptions<OnspringOptionsSetup>();
builder.Services.AddSingleton<IOnspringService, OnspringService>();

builder.Services.ConfigureOptions<ResourceMonitorOptionsSetup>();
builder.Services.AddHostedService<ResourceMonitor>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();