using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public record VersionedTable(Table? Table, int Version)
{
  private void CheckVersion(int incoming)
  {
    if (incoming != Version + 1)
    {
      throw new IllegalStateException("version mismatch");
    }
  }

  private ActionMeta NextMeta => new(NextVersion, DateTime.UtcNow);

  public (VersionedTable, TableAction<InitTable>) Init(IRules rules, int numberOfSeats) =>
    Apply(new InitTable(rules, numberOfSeats));

  public (VersionedTable, TableAction<StartMatch>) StartMatch()
  {
    var cards = Table?.Rules.Deck.Shuffle(Random.Shared) ?? throw Err.Table.NotInitialized;
    return Apply(new StartMatch(cards));
  }

  public (VersionedTable, TableAction<Reserve>) Reserve(Seat seat, bool reserved) =>
    Apply(new Reserve(seat, reserved));

  public (VersionedTable, TableAction<Declare>) Declare(Seat seat, IGameMode mode) =>
    Apply(new Declare(seat, mode));

  public (VersionedTable, TableAction<PlayCard>) PlayCard(Seat seat, Card card) =>
    Apply(new PlayCard(seat, card));

  private int NextVersion => Version + 1;

  private (VersionedTable, TableAction<T>) Progress<T>(Table next, T payload)
    where T : ITableActionPayload
  {
    var action = CreateAction(payload);
    return (new(next, action.Meta.Version), action);
  }

  private TableAction<T> CreateAction<T>(T payload)
    where T : ITableActionPayload
  {
    return new(payload, NextMeta);
  }

  private (VersionedTable, TableAction<T>) Apply<T>(T payload)
    where T : ITableActionPayload => Apply(CreateAction(payload));

  public (VersionedTable, TableAction<T>) Apply<T>(TableAction<T> action)
    where T : ITableActionPayload
  {
    var (payload, meta) = action;
    CheckVersion(meta.Version);
    if (payload is InitTable init)
    {
      if (Table != null)
      {
        throw new IllegalStateException("table already initialized");
      }
      var table = new Table(init.Rules, init.NumberOfPlayers);
      return Progress(table, payload);
    }
    if (Table is null)
    {
      throw new IllegalStateException("table not initialized");
    }
    var nextTable = payload switch
    {
      StartMatch match => Table.StartNextMatch(match.Cards),
      Reserve res => Table.Reserve(res.Seat, res.Reserved),
      Declare dec => Table.Declare(dec.Seat, dec.Mode),
      PlayCard ply => Table.PlayCard(ply.Seat, ply.Card),
      _ => throw new ArgumentException("unknown action")
    };
    return Progress(nextTable, payload);
  }
}
