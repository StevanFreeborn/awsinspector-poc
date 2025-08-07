
namespace AwsInspectorPoc.API.Telemetry;

internal static class Extensions
{
  public static IHostApplicationBuilder AddTelemetry(this IHostApplicationBuilder builder)
  {
    var seq = new SeqOptions();
    builder.Configuration.GetSection(nameof(SeqOptions)).Bind(seq);

    if (seq.IsEnabled is false)
    {
      return builder;
    }

    builder.Services.AddSingleton<Instrumentation>();

    var instrumentation = builder.Services
      .BuildServiceProvider()
      .GetRequiredService<Instrumentation>();

    builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource =>
      {
        resource.AddService(instrumentation.SourceName, instrumentation.SourceVersion);
        resource.AddAttributes(new Dictionary<string, object>
        {
          ["service.name"] = instrumentation.SourceName,
          ["service.version"] = instrumentation.SourceVersion,
          ["service.instance.id"] = Environment.MachineName,
          ["service.namespace"] = "AwsInspectorPoc",
          ["service.environment"] = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "production",
        });
      })
      .WithLogging(lb =>
      {
        lb.AddOtlpExporter(o =>
        {
          o.Protocol = OtlpExportProtocol.HttpProtobuf;
          o.Endpoint = new Uri(seq.LogEndpoint);
          o.Headers = seq.AuthHeader;
        });
      })
      .WithTracing(tb =>
      {
        tb.AddSource(instrumentation.SourceName);
        tb.AddAspNetCoreInstrumentation();
        tb.AddHttpClientInstrumentation();
        tb.AddOtlpExporter(o =>
        {
          o.Protocol = OtlpExportProtocol.HttpProtobuf;
          o.Endpoint = new Uri(seq.TraceEndpoint);
          o.Headers = seq.AuthHeader;
        });
      });

    return builder;
  }
}