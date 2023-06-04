using Doppelkopf.API;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Authentication;

public class AllowAllLoginHandler : ILoginHandler
{
  public Task<UserData> Login(UserId id, string secret, string? name = null) =>
      Task.FromResult(new UserData(id, name ?? id));
}
