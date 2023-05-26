using Microsoft.AspNetCore.Authentication;

namespace Doppelkopf.Server.Authentication;

public class DebugAuthenticationOptions : AuthenticationSchemeOptions
{
  public const string Schema = "debug_schema";
}
