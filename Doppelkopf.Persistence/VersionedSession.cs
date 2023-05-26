using Doppelkopf.Cards;
using Doppelkopf.Conf;
using Doppelkopf.Contracts;
using Doppelkopf.Errors;
using Doppelkopf.Sessions;

namespace Doppelkopf.Persistence;

public record VersionedSession(Session Session, int Version)
{
  public VersionedSession(InitTable init) : this(new Session(init.Configuration, init.NumberOfPlayers), 0)
  {
  }

  private void CheckVersion(int incoming)
  {
    if (incoming != Version + 1)
    {
      throw new IllegalStateException("version mismatch");
    }
  }

  private ActionMeta NextMeta => new(NextVersion, DateTime.UtcNow);

  public (VersionedSession, TableAction<StartGame>) StartGame()
  {
    var cards = Session.Configuration.Deck.Shuffle(Random.Shared) ?? throw Err.Table.NotInitialized;
    return Apply(new StartGame(cards));
  }

  public (VersionedSession, TableAction<Reserve>) Reserve(Seat seat, bool reserved) => Apply(new Reserve(seat, reserved));

  public (VersionedSession, TableAction<Declare>) Declare(Seat seat, IContract contract) =>
      Apply(new Declare(seat, contract));

  public (VersionedSession, TableAction<PlayCard>) PlayCard(Seat seat, Card card) => Apply(new PlayCard(seat, card));

  private int NextVersion => Version + 1;

  private (VersionedSession, TableAction<T>) Progress<T>(Session next, T payload)
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

  private (VersionedSession, TableAction<T>) Apply<T>(T payload)
      where T : ITableActionPayload =>
      Apply(CreateAction(payload));

  public (VersionedSession, TableAction<T>) Apply<T>(TableAction<T> action)
      where T : ITableActionPayload
  {
    var (payload, meta) = action;
    if (payload is InitTable)
    {
      throw new ArgumentException("init must be used to create table");
    }

    CheckVersion(meta.Version);
    var nextTable = payload switch
    {
        StartGame game => Session.StartNextGame(game.Cards),
        Reserve res => Session.Reserve(res.Seat, res.Reserved),
        Declare dec => Session.Declare(dec.Seat, dec.Contract),
        PlayCard ply => Session.PlayCard(ply.Seat, ply.Card),
        _ => throw new ArgumentException("unknown action")
    };
    return Progress(nextTable, payload);
  }
}
