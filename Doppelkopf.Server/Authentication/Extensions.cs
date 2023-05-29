using System.Security.Claims;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Authentication;

public static class Extensions
{
  public static UserData AuthenticatedUser(this HttpContext context)
  {
    var id = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
    if (id is null)
    {
      throw new ArgumentException("given context has no authenticated user");
    }
    var name = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? id;
    return new(new(id), name);
  }
}
