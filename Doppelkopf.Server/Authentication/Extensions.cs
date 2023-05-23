using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Authentication;

public static class Extensions
{
    public static UserId AuthenticatedUserId(this HttpContext context)
    {
        var name = context.User.Identity?.Name;
        if (name is null)
        {
            throw new ArgumentException("given context has no authenticated user");
        }
        return new(name);
    }
}
