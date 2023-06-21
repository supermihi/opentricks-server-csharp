using System.Net;
using Doppelkopf.Server.TableActions;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Model;

/// <summary>
///
/// </summary>
/// <param name="Meta"></param>
/// <param name="Users">Table users, including the owner.</param>
/// <param name="Session"></param>
public sealed record Table(TableMeta Meta, TableUsers Users, int Version,
  Session? Session)
{
  public static Table Init(TableMeta meta, IEnumerable<UserId>? invitedUsers = null)
  {
    var users = TableUsers.Init(new[] { meta.Owner }.Concat(invitedUsers ?? Enumerable.Empty<UserId>()));
    return new(meta, users, 0, null);
  }

  public TableActionResult AddUser(UserId user, bool ready)
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
    var newTable = this with { Users = Users.Add(user, ready), Version = NextVersion };
    return new(newTable, new JoinTableEvent(user));
  }

  public TableActionResult MarkReady(UserId user)
  {
    AssertNotStarted();
    AssertUserAtTable(user);
    if (Users.IsReady(user))
    {
      throw new ArgumentException("user already ready");
    }
    var newTable = this with { Users = Users.SetReady(user), Version = NextVersion };
    return new(newTable, new MarkedReadyEvent(user));
  }

  public TableActionResult Start(UserId player, Random? random = null)
  {
    AssertNotStarted();
    if (Users is not { Count: >= Constants.NumberOfPlayers })
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
    var newTable = this with
    {
      Session = Session.Init(Meta.Rules.ToConfiguration(), Users.Count, random ?? Random.Shared),
      Version = NextVersion
    };
    return new TableActionResult(newTable, new StartTableEvent(player));
  }

  private void AssertUserAtTable(UserId id)
  {
    if (!Users.Contains(id))
    {
      throw new ArgumentException("user not at table");
    }
  }

  public TableActionResult Reserve(UserId user, bool reservation)
  {
    var (session, seat) = GetSessionAndSeat(user);
    var newTable = this with { Session = session.Reserve(seat, reservation), Version = NextVersion };
    var result = new TableActionResult(newTable, new ReserveEvent(user, reservation));
    return newTable.AuctionFinishedEventIfFinished() is { } contracted ? result.Add(contracted) : result;
  }

  private AuctionFinishedEvent? AuctionFinishedEventIfFinished()
  {
    var game = Session!.Game;
    return game.Contract is not null ? new AuctionFinishedEvent() : null;
  }

  public TableActionResult Declare(UserId user, string contractId)
  {
    var (session, seat) = GetSessionAndSeat(user);
    var contract = session.Configuration.Contracts.FirstOrDefault(c => c.Id == contractId);
    if (contract is null)
    {
      throw new ArgumentException("contract not found");
    }
    var newTable = this with { Session = session.Declare(seat, contract), Version = NextVersion };
    var result = new TableActionResult(newTable, new DeclareEvent(user, contract.Type));
    return newTable.AuctionFinishedEventIfFinished() is { } contracted ? result.Add(contracted) : result;
  }

  public TableActionResult PlayCard(UserId user, string cardId)
  {
    var (session, seat) = GetSessionAndSeat(user);
    var card = session.Configuration.Cards.GetById(cardId);
    var (newSession, finishedTrick, finishedGame) = session.PlayCardAndProceed(seat, card);
    var newTable = this with { Session = newSession, Version = NextVersion };
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
