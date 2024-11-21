using Doppelkopf.Core.Games;
using Doppelkopf.Errors;

namespace Doppelkopf.API;

public interface IPlayerClient
{
  Task<ErrorCode?> Play(PlayerAction action);
}
