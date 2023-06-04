using Doppelkopf.API;
using Doppelkopf.Games;
using Doppelkopf.Server.Model;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Interface;

public static class Extensions
{
  public static TableState ToJsonTable(this Table table, UserId maskFor)
  {
    return new TableState(
      Id: table.Meta.Id,
      Name: table.Meta.Name,
      Owner: table.Meta.Owner,
      Players: table.Users.Select(u => u.Id).ToArray(),
      Version: table.Version,
      RuleSet: table.Meta.Rules.RuleSet,
      MaxSeats: table.Meta.Rules.MaxSeats,
      Started: table.IsStarted
    );
  }

  public static JsonTricks ToJsonTricks(this TrickTaking trickTaking)
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

  public static GameState ToJsonGame(this Game game, Player? maskFor)

  {
    var cards = JsonByPlayer.FromByPlayer(
      game.Cards,
      (player, cards) => player == maskFor
          ? (IReadOnlyList<string>)cards.Select(c => c.Id).ToArray()
          : new string[] { });
    var auction = game.TrickTaking is null ? game.Auction.ToJsonAuction(maskFor) : null;
    var contract = game.TrickTaking?.Contract.Id;
    var tricks = game.TrickTaking is null or { IsFinished: true } ? null : game.TrickTaking.ToJsonTricks();
    return new(cards, auction, contract, tricks);
  }

  public static AuctionState ToJsonAuction(this Auction auction, Player? maskFor)
  {
    return new(
      JsonByPlayer.Create(
        player =>
        {
          var isReserved = auction.Reservations.TryGet(player, out var reserved) ? (bool?)reserved : null;
          return new PlayerAuctionState(isReserved, auction.HasDeclared(player));
        }));
  }

  public static SessionState ToJsonSession(this Session session, Seat maskFor)
  {
    var player = session.AtSeat(maskFor);
    return new(
      PlayerIndexes: session.ActiveSeats.Select(seat => seat.Position).ToArray(),
      IsFinished: session.IsFinished,
      CurrentGame: session.Game.ToJsonGame(player)
    );
  }
}
