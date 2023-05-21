using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Doppelkopf.Server.Authentication;

public class DebugAuthenticationHandler : AuthenticationHandler<DebugAuthenticationOptions>
{
  public DebugAuthenticationHandler(
    IOptionsMonitor<DebugAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock
  )
    : base(options, logger, encoder, clock) { }

  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    var user = Context.Request.Headers[HeaderNames.Authorization].ToString();
    if (string.IsNullOrEmpty(user))
    {
      return Task.FromResult(AuthenticateResult.Fail("no user"));
    }
    IIdentity id = new GenericIdentity(user);
    var claimsPrincipal = new ClaimsPrincipal(id);
    var ticket = new AuthenticationTicket(claimsPrincipal, DebugAuthenticationOptions.Schema);
    return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}
