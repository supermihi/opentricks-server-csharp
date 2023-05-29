using Doppelkopf.Server.Model;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Interface;

public sealed record JsonTable(string Id,
  string Name,
  string Owner,
  IReadOnlyList<string> Players,
  int Version,
  bool Started,
  RuleSet RuleSet,
  int MaxSeats)
{
  public static JsonTable FromTable(Table table, UserId maskFor)
  {
    return new JsonTable(
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
}

public sealed record JsonSession(IReadOnlyList<int> PlayerIndexes, bool IsFinished, JsonGame? CurrentGame)
{
  public static JsonSession FromSession(Session session, Seat maskFor)
  {
    var player = session.AtSeat(maskFor);
    return new(
      PlayerIndexes: session.ActiveSeats.Select(seat => seat.Position).ToArray(),
      IsFinished: session.IsFinished,
      CurrentGame: session.Game is null ? null : JsonGame.FromGame(session.Game, player)
    );
  }
}
