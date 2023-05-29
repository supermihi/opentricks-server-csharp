using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Doppelkopf.Server.Authentication;

public class DebugAuthenticationHandler : AuthenticationHandler<DebugAuthenticationOptions>
{
  public DebugAuthenticationHandler(IOptionsMonitor<DebugAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock)
      : base(options, logger, encoder, clock)
  { }

  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    var authenticatedUser = UserCookie.Get(Context.Request.Cookies);
    if (authenticatedUser is null)
    {
      var user = Context.Request.Headers[HeaderNames.Authorization].ToString();
      if (string.IsNullOrEmpty(user))
      {
        return Task.FromResult(AuthenticateResult.Fail("no user"));
      }
      authenticatedUser = new(new(user), user);
    }
    var principal = Claims.CreateClaimsPrincipal(authenticatedUser, DebugAuthenticationOptions.Schema);
    var ticket = new AuthenticationTicket(principal, DebugAuthenticationOptions.Schema);
    return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}
