using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Games;
using Doppelkopf.Errors;

namespace Doppelkopf.Cli;

internal sealed class LockingSynchronousClient(IGame game, Player player) : IDoppelkopfApi
{
  public Task<ErrorCode?> Play(PlayerAction action)
  {
    lock (game)
    {
      try
      {
        game.Play(player, action);
        return Task.FromResult<ErrorCode?>(null);
      }
      catch (InvalidMoveException ime)
      {
        return Task.FromResult<ErrorCode?>(ime.ErrorCode);
      }
    }
  }
}
