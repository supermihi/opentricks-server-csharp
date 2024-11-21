using Doppelkopf.API.Errors;

namespace Doppelkopf.API;

public interface IDoppelkopfApi
{
  Task<ErrorCode?> Play(PlayerAction action);
}
