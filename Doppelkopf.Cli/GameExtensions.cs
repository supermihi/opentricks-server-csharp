using System.Collections.Immutable;
using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Cli;

public static class GameExtensions
{
  public static GameView ToPlayerGameView(this IGame game, Player player) =>
    new(
      player,
      game.Cards[player],
      game.Phase,
      game.GetTurn(),
      game.Phase == GamePhase.Auction
        ? new AuctionView(Player.One, game.Declarations.Select(d => d.HoldId is not null).ToArray())
        : null,
      game.Phase == GamePhase.Auction
        ? null
        : new TrickTakingView(
          new ContractView(
            game.AuctionResult!.Hold?.Id,
            game.AuctionResult!.Declarer,
            game.AuctionResult!.IsCompulsorySolo),
          game.CurrentTrick?.ToTrickView(),
          game.CompleteTricks.Any() ? game.CompleteTricks[^1].ToTrickView() : null,
          Array.Empty<BidView>() // tODO
        )
    );

  public static TrickView ToTrickView(this Trick trick) =>
    new(
      trick.Leader,
      trick.Cards.ToList(),
      trick.Index,
      null);

  public static TrickView ToTrickView(this CompleteTrick trick) =>
    new(
      trick.Leader,
      trick.Cards.InOrder(trick.Leader).ToImmutableArray(),
      trick.Index,
      trick.Winner);
}
