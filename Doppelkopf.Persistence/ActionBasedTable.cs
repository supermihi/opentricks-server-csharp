using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public class ActionBasedTable
{
  public Table? Table { get; private set; }
  public int Version { get; private set; }

  public ActionBasedTable()
  {
    Version = 0;
  }

  private void CheckVersion(int incoming)
  {
    if (incoming != Version + 1)
    {
      throw new IllegalStateException("version mismatch");
    }
  }

  private int IncreaseVersion() => Version = NextVersion;

  public InitTableAction Init(IRules rules, int numberOfSeats) =>
      Apply(new InitTableAction(rules, numberOfSeats, NextVersion));

  public StartMatchAction StartMatch()
  {
    var cards = Table?.Rules.Deck.Shuffle(Random.Shared) ?? throw Err.Table.NotInitialized;
    return Apply(new StartMatchAction(cards, NextVersion));
  }

  public ReserveAction Reserve(Seat seat, bool reserved) =>
      Apply(new ReserveAction(seat, reserved, NextVersion));

  public DeclareAction Declare(Seat seat, IGameMode mode) =>
      Apply(new DeclareAction(seat, mode, NextVersion));

  public PlayCardAction PlayCard(Seat seat, Card card) =>
      Apply(new PlayCardAction(seat, card, NextVersion));

  private int NextVersion => Version + 1;

  public T Apply<T>(T action)
      where T : ITableAction
  {
    CheckVersion(action.Version);
    IncreaseVersion();
    if (action is InitTableAction init)
    {
      if (Table != null)
      {
        Version -= 1;
        throw new IllegalStateException("table already initialized");
      }
      Table = new(init.Rules, init.NumberOfPlayers);
      return action;
    }
    if (Table is null)
    {
      Version -= 1;
      throw new IllegalStateException("table not initialized");
    }
    try
    {
      Table = action switch
      {
          StartMatchAction match => Table.StartNextMatch(match.Cards),
          ReserveAction res => Table.Reserve(res.Seat, res.Reserved),
          DeclareAction dec => Table.Declare(dec.Seat, dec.Mode),
          PlayCardAction ply => Table.PlayCard(ply.Seat, ply.Card),
          _ => throw new ArgumentException("unknown action")
      };
    }
    catch
    {
      Version -= 1;
      throw;
    }
    return action;
  }
}