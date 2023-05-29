using System.Security.Claims;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Authentication;

public static class Claims
{
  public static ClaimsIdentity CreateClaimsIdentity(UserData user, string authenticationType)
  {
    return new ClaimsIdentity(
      new Claim[] { new(ClaimTypes.Sid, user.Id.Id), new(ClaimTypes.Name, user.Name) },
      authenticationType);
  }

  public static ClaimsPrincipal CreateClaimsPrincipal(UserData user, string authenticationType)
  {
    var identity = CreateClaimsIdentity(user, authenticationType);
    return new ClaimsPrincipal(identity);
  }
}
