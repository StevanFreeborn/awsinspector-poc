namespace AwsInspectorPoc.API.Authentication;

class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
{
  public const string SchemeName = "Basic";
  private readonly IOptionsMonitor<BasicAuthOptions> _basicOptions;

  public BasicAuthentication(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptionsMonitor<BasicAuthOptions> basicOptions
  ) : base(options, logger, encoder)
  {
    _basicOptions = basicOptions;
  }

  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    var endpoint = Context.GetEndpoint();

    if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is not null)
    {
      return Task.FromResult(AuthenticateResult.NoResult());
    }

    var authHeader = Request.Headers.Authorization.FirstOrDefault();

    if (authHeader is null)
    {
      return Task.FromResult(AuthenticateResult.Fail("Authorization header is missing"));
    }

    var startsWithBasic = authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase);

    if (startsWithBasic is false)
    {
      return Task.FromResult(AuthenticateResult.Fail("Authorization header is not Basic"));
    }

    var headerParts = authHeader.Split(' ', 2);

    if (headerParts.Length < 2)
    {
      return Task.FromResult(AuthenticateResult.Fail("Invalid authorization header"));
    }

    var encodedCredentials = headerParts[1];

    var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
    var credentials = decodedCredentials.Split(':', 2);

    if (credentials.Length < 2)
    {
      return Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));
    }

    var username = credentials[0];
    var password = credentials[1];

    if (
      username != _basicOptions.CurrentValue.Username ||
      password != _basicOptions.CurrentValue.Password
    )
    {
      return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));
    }

    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, username),
      new Claim(ClaimTypes.Name, username),
    };

    var identity = new ClaimsIdentity(claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);

    return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}