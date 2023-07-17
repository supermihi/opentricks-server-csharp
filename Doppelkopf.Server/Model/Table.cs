using System.Net;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Games;
using Doppelkopf.Server.TableActions;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Model;

/// <summary>
///
/// </summary>
/// <param name="Meta"></param>
/// <param name="Users">Table users, including the owner.</param>
/// <param name="Session"></param>
public class Table
{
  public TableMeta Meta { get; }
  public int Version { get; private set; }
  public Session? Session { get; private set; }

  public Table(TableMeta meta, IEnumerable<UserId>? invitedUsers = null)
  {
    Meta = meta;
    Users = TableUsers.Init(new[] { meta.Owner }.Concat(invitedUsers ?? Enumerable.Empty<UserId>()));
  }

  public TableUsers Users { get; private set; }

  public JoinTableEvent AddUser(UserId user, bool ready)
  {
    AssertNotStarted();
    if (Users.Contains(user))
    {
      throw new UserInputException(HttpStatusCode.BadRequest, "already_at_table", "user already at table");
    }
    if (Users.Count + 1 > Meta.Rules.MaxSeats)
    {
      throw new UserInputException(HttpStatusCode.BadRequest, "table_full", "table is full");
    }
    Users = Users.Add(user, ready);
    BumpVersion();
    return new JoinTableEvent(user);
  }

  private void BumpVersion()
  {
    Version++;
  }

  public MarkedReadyEvent MarkReady(UserId user)
  {
    AssertNotStarted();
    AssertUserAtTable(user);
    if (Users.IsReady(user))
    {
      throw new ArgumentException("user already ready");
    }
    Users = Users.SetReady(user);
    BumpVersion();
    return new MarkedReadyEvent(user);
  }

  public StartTableEvent Start(UserId player, Random? random = null)
  {
    AssertNotStarted();
    if (Users is not { Count: >= Core.Rules.NumPlayers })
    {
      throw new UserInputException(HttpStatusCode.BadRequest, "too_few_users", "not enough users to start table");
    }
    if (player != Meta.Owner)
    {
      throw new UserInputException(HttpStatusCode.Forbidden, "not_owner", "only the table owner may start the table");
    }
    if (Users.Any(u => !Users.IsReady(u) && u != player))
    {
      throw new UserInputException(HttpStatusCode.BadRequest, "not_ready", "not everybody is ready");
    }
    Session = new Session(
      Meta.Rules.SessionConfiguration(),
      Meta.Rules.GameConfiguration().CreateGameFactory(123),
      numberOfSeats: Users.Count);

    return new StartTableEvent(player);
  }

  private void AssertUserAtTable(UserId id)
  {
    if (!Users.Contains(id))
    {
      throw new ArgumentException("user not at table");
    }
  }

  private AuctionFinishedEvent? AuctionFinishedEventIfFinished()
  {
    var game = Session!.CurrentGame;
    return game.Phase == GamePhase.TrickTaking ? new AuctionFinishedEvent() : null;
  }

  public TableActionResult Declare(UserId user, string contractId)
  {
    var (session, seat) = GetSessionAndSeat(user);
    var contract = session.CurrentGame.Contracts.DeclarableContracts.FirstOrDefault(c => c.Id == contractId);
    if (contract is null)
    {
      throw new ArgumentException("contract not found");
    }
    session.MakeReservation(seat, contract);
    BumpVersion();
    var result = new TableActionResult(this,new DeclareEvent(user, contract.Type));
    return AuctionFinishedEventIfFinished() is { } contracted ? result.Add(contracted) : result;
  }

  public TableActionResult PlayCard(UserId user, Card card)
  {
    var (session, seat) = GetSessionAndSeat(user);
    session.PlayCard(seat, card);
    BumpVersion();
    var result = new TableActionResult(newTable, new PlayCardEvent(user, card));
    if (finishedTrick != null)
    {
      result = result.Add(new FinishTrickEvent(newTable.UserForPlayer(finishedTrick.Winner)));
    }
    if (finishedGame != null)
    {
      result = result.Add(new GameFinishedEvent(newSession.IsFinished));
    }
    return result;
  }

  private int NextVersion => Version + 1;

  private void AssertNotStarted()
  {
    if (IsStarted)
    {
      throw new UserInputException(HttpStatusCode.BadRequest, "table_started", "table already started");
    }
  }

  private (Session, Seat) GetSessionAndSeat(UserId user)
  {
    if (Session is null)
    {
      throw new ArgumentException("table not started");
    }
    AssertUserAtTable(user);
    return (Session, Users.SeatOf(user));
  }

  public bool IsStarted => Session != null;
}
