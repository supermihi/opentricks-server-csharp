using Doppelkopf.API;
using Doppelkopf.API.Errors;
using Doppelkopf.Core.Games;

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
