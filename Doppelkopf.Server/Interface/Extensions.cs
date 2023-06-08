using System.Collections.Immutable;
using Doppelkopf.API;
using Doppelkopf.Cards;
using Doppelkopf.Games;
using Doppelkopf.Server.Model;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Interface;

public static class Extensions
{
  public static TableState ToTableState(this Table table, UserId maskFor)
  {
    return new TableState(
      Id: table.Meta.Id,
      Name: table.Meta.Name,
      Owner: table.Meta.Owner,
      Players: table.Users.Select(u => u.Id).ToArray(),
      Version: table.Version,
      RuleSet: table.Meta.Rules.RuleSet,
      MaxSeats: table.Meta.Rules.MaxSeats,
      Started: table.IsStarted,
      Session: table.Session?.ToTableState(table.Users.SeatOf(maskFor))
    );
  }

  public static JsonTricks ToTricksState(this TrickTaking trickTaking)
  {
    {
      var currentTrick = JsonByPlayer.FromInTurns(
        trickTaking.CurrentTrick!.Cards,
        (_, card) => card.Id,
        null);
      var tricksWon = JsonByPlayer.Create(p => trickTaking.CompleteTricks.Count(trick => trick.Winner == p));
      return new(currentTrick, tricksWon);
    }
  }

  public static GameState ToGameState(this Game game, Player? maskFor)

  {
    var cards = Enum.GetValues<Player>()
        .Select(
          player => (player == maskFor ? game.Cards[player] : ImmutableList<Card>.Empty).Select(c => c.Id).ToArray())
        .ToArray();
    var contract = game.TrickTaking?.Contract.Id;
    var auction = game.TrickTaking is null ? game.Auction.ToAuctionState(maskFor) : null;
    var tricks = game.TrickTaking is null or { IsFinished: true } ? null : game.TrickTaking.ToTricksState();
    return new(cards, auction, contract, tricks);
  }

  public static AuctionState ToAuctionState(this Auction auction, Player? maskFor)
  {
    return new(
      JsonByPlayer.Create(
        player =>
        {
          var isReserved = auction.Reservations.TryGet(player, out var reserved) ? (bool?)reserved : null;
          return new PlayerAuctionState(isReserved, auction.HasDeclared(player));
        }));
  }

  public static SessionState ToTableState(this Session session, Seat maskFor)
  {
    var player = session.AtSeat(maskFor);
    return new(
      PlayerIndexes: session.ActiveSeats.Select(seat => seat.Position).ToArray(),
      IsFinished: session.IsFinished,
      CurrentGame: session.Game.ToGameState(player)
    );
  }
}
