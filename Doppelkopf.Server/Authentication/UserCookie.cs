using System.Text.Json;
using System.Text.Json.Serialization;
using Doppelkopf.API;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Authentication;

public static class UserCookie
{
  private sealed record UserCookieData([property: JsonPropertyName("id"), JsonRequired]
    string Id,
    [property: JsonPropertyName("name"), JsonRequired]
    string Name,
    [property: JsonPropertyName("secret"), JsonRequired]
    string Secret);

  private const string CookieName = "OpenTricks.User";

  public static UserData? Get(IRequestCookieCollection cookies)
  {
    if (cookies.TryGetValue(CookieName, out var cookieValue))
    {
      try
      {
        var cookieData = JsonSerializer.Deserialize<UserCookieData>(cookieValue);
        if (cookieData is not null)
        {
          return new(new(cookieData.Id), cookieData.Name);
        }
      }
      catch (JsonException)
      {
        return null;
      }
    }
    return null;
  }

  public static void Set(IResponseCookies cookies, UserId id, string name, string secret)
  {
    cookies.Append(
      CookieName,
      JsonSerializer.Serialize(new UserCookieData(id, name, secret)),
      s_options
    );
  }

  private static readonly CookieOptions s_options = new() { HttpOnly = false, Secure = false };
}
