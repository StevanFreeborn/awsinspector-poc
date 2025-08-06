namespace AwsInspectorPoc.API.Options;

internal sealed class BasicAuthOptions
{
  public string Username { get; init; } = "admin";
  public string Password { get; init; } = "password";
}