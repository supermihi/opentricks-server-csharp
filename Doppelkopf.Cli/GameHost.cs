using System.Collections.Immutable;
using Doppelkopf.API;
using Doppelkopf.Bot;
using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Cli;

internal sealed class GameForPlayer(IGame game, Player player) : IGameForPlayer
{
  public Task PlayCard(Card card) => Task.FromResult(game.PlayCard(player, card));

  public Task DeclareHold(string? holdId)
  {
    if (holdId is null)
    {
      game.DeclareOk(player);
    }
    else
    {
      var hold = game.Modes.Holds.Single(h => h.Id == holdId);
      game.DeclareHold(player, hold);
    }

    return Task.CompletedTask;
  }

  public Task PlaceBid(Bid bid)
  {
    game.PlaceBid(player, bid);
    return Task.CompletedTask;
  }
}

public class SingleGameHost(IGameFactory gameFactory)
{
  private readonly IGame _game = gameFactory.CreateGame(ByPlayer.Init(false));
  private readonly Dictionary<Player, IBot> _bots = [];

  public void AddBot(Player player, IBot bot) => _bots[player] = bot;

  public async Task RunToCompletion()
  {
    while (_game.Phase != GamePhase.Finished)
    {
      foreach (var player in Enum.GetValues<Player>())
      {
        var view = _game.ToPlayerGameView(player);
        if (_bots.TryGetValue(player, out var bot))
        {
          await bot.OnGameStateChanged(view, new GameForPlayer(_game, player));
        }
      }
    }
    var result = _game.Evaluate();
    Console.WriteLine(
      $"finished! winner: {result.Winner}; mode: {_game.AuctionResult!.Hold?.Id}, {result.ScoreByParty()}");
    Console.WriteLine($"base: {result.BaseScore}");
    foreach (var ep in result.ExtraPoints)
    {
      Console.WriteLine($"extra point: {ep}");
    }
    Console.WriteLine(result.Parties);
  }
}

public static class GameExtensions
{
  public static GameView ToPlayerGameView(this IGame game, Player player) =>
    new(
      player,
      game.Cards[player],
      game.Turn,
      game.Phase == GamePhase.Auction
        ? new AuctionView(game.Declarations.Select(d => d.Hold is not null).ToArray())
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
