using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Scoring;

namespace Doppelkopf.Cli;

internal sealed class PlayerClient(IGame game, Player player, Action? onAction) : IPlayerClient
{
  public Task PlayCard(Card card)
  {
    lock (game)
    {
      game.PlayCard(player, card);
      onAction?.Invoke();
    }
    return Task.CompletedTask;
  }

  public Task DeclareHold(string? holdId)
  {
    if (holdId is null)
    {
      lock (game)
      {
        game.DeclareOk(player);
        onAction?.Invoke();
      }
    }
    else
    {
      var hold = game.Configuration.GameModes.Holds.Single(h => h.Id == holdId);
      lock (game)
      {
        game.DeclareHold(player, hold);
        onAction?.Invoke();
      }
    }

    return Task.CompletedTask;
  }

  public Task PlaceBid(Bid bid)
  {
    lock (game)
    {
      game.PlaceBid(player, bid);
      onAction?.Invoke();
    }
    return Task.CompletedTask;
  }
}
