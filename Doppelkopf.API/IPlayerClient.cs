using Doppelkopf.Core.Games;
using Doppelkopf.Errors;

namespace Doppelkopf.API;

public interface IDoppelkopfApi
{
  Task<ErrorCode?> Play(PlayerAction action);
}
