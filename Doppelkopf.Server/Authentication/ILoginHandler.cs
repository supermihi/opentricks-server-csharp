using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Authentication;

public interface ILoginHandler
{
  Task<UserData> Login(UserId id, string secret, string? name = null);
}
