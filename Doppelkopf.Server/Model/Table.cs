using Doppelkopf.API;
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
  public static Table Init(TableMeta meta, IEnumerable<UserId>? invitedUsers)
  {
    var users = TableUsers.Init(new[] { meta.Owner }.Concat(invitedUsers ?? Enumerable.Empty<UserId>()));
    return new(meta, users, 0, null);
  }

  public TableActionResult Join(UserId user)
  {
    AssertNotStarted();
    if (Users.Count + 1 > Meta.Rules.MaxSeats)
    {
      throw new ArgumentException("table is full");
    }
    var newTable = this with { Users = Users.Add(user), Version = NextVersion };
    return new(newTable, TableEvent.JoinTable(user));
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
    return new(newTable, TableEvent.MarkReady(user));
  }

  public TableActionResult Start(UserId player, Random? random = null)
  {
    AssertNotStarted();
    if (Users is not { Count: >= Constants.NumberOfPlayers, AreAllReady: true })
    {
      throw new ArgumentException("not all users ready");
    }
    if (player != Meta.Owner)
    {
      throw new ArgumentException("not your table");
    }
    var newTable = this with
    {
      Session = Session.Init(Meta.Rules.ToConfiguration(), Users.Count, random ?? Random.Shared),
      Version = NextVersion
    };
    return new TableActionResult(newTable, TableEvent.Start());
  }

  private void AssertUserAtTable(UserId user)
  {
    if (!Users.Contains(user))
    {
      throw new ArgumentException("user not at table");
    }
  }

  public TableActionResult Reserve(UserId user, bool reservation)
  {
    var (session, seat) = GetSessionAndSeat(user);
    var newTable = this with { Session = session.Reserve(seat, reservation), Version = NextVersion };
    var result = new TableActionResult(newTable, TableEvent.Reserve(user, reservation));
    return newTable.ContractedEventIfContractDefined() is { } contracted ? result.Add(contracted) : result;
  }

  private TableEvent? ContractedEventIfContractDefined()
  {
    var game = Session!.Game;
    if (game.Contract is { } contract)
    {
      var declarer = game.PartyData.Declarer;
      return TableEvent.Contracted(contract.Id, declarer is null ? null : this.UserForPlayer(declarer.Value));
    }
    return null;
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
    var result = new TableActionResult(newTable, TableEvent.Declare(user, contract.Type));
    return newTable.ContractedEventIfContractDefined() is { } contracted ? result.Add(contracted) : result;
  }

  public TableActionResult PlayCard(UserId user, string cardId)
  {
    var (session, seat) = GetSessionAndSeat(user);
    var card = session.Configuration.Cards.GetById(cardId);
    var (newSession, finishedTrick, finishedGame) = session.PlayCardAndProceed(seat, card);
    var newTable = this with { Session = newSession, Version = NextVersion };
    var result = new TableActionResult(newTable, TableEvent.PlayCard(user, card));
    if (finishedTrick != null)
    {
      result = result.Add(TableEvent.FinishTrick(newTable.UserForPlayer(finishedTrick.Winner)));
    }
    if (finishedGame != null)
    {
      result = result.Add(TableEvent.FinishGame(sessionFinished: newSession.IsFinished));
    }
    return result;
  }

  private int NextVersion => Version + 1;

  private void AssertNotStarted()
  {
    if (IsStarted)
    {
      throw new ArgumentException("table already started");
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
