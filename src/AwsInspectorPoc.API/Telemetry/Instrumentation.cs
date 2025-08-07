using System.Diagnostics;

internal sealed class Instrumentation
{
  private const string SourceNameValue = "AwsInspectorPoc.API";
  private const string SourceVersionValue = "0.0.0";
  public string SourceName { get; } = SourceNameValue;
  public string SourceVersion { get; } = SourceVersionValue;
  public ActivitySource Source { get; } = new ActivitySource(SourceNameValue, SourceVersionValue);

  public void Dispose()
  {
    Source.Dispose();
  }
}