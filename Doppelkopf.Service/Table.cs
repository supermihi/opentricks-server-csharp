using System.Collections.Immutable;
using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Errors;
using Doppelkopf.Sessions;

namespace Doppelkopf.Service;

internal sealed class Table : ITable
{
  private readonly ISessionFactory _sessionFactory;

  public Table(TableData data, SessionConfiguration initialSessionConfig,
    GameConfiguration initialGameConfig,
    ISessionFactory sessionFactory)
  {
    Data = data;
    SessionConfig = initialSessionConfig;
    GameConfig = initialGameConfig;
    _sessionFactory = sessionFactory;
    AdditionalMembers = [];
    SetReady(Data.Owner, true);
  }
  public ImmutableArray<UserId> AdditionalMembers { get; private set; }
  public TableData Data { get; }
  public SessionConfiguration SessionConfig { get; private set; }
  public GameConfiguration GameConfig { get; private set; }
  public ISession? Session { get; private set; }

  private readonly HashSet<UserId> _readyUsers = new();

  public bool IsReady(UserId user)
  {
    CheckIsMember(user);
    return _readyUsers.Contains(user);
  }

  public void SetReady(UserId user, bool value)
  {
    CheckNotStarted();
    if (value)
    {
      _readyUsers.Add(user);
    }
    else
    {
      _readyUsers.Remove(user);
    }
  }

  public Task Start()
  {
    if (!Data.Members.All(IsReady))
    {
      throw ErrorCodes.NotReady.ToException();
    }
    Session = _sessionFactory.CreateSession(SessionConfig, GameConfig);
    return Task.CompletedTask;
  }

  public Task<PlayCardResult> PlayCard(UserId user, Card card)
  {
    var session = CheckStarted();
    var player = ActivePlayer(user);
    var result = session.CurrentGame.PlayCard(player, card);
    return Task.FromResult(result);
  }


  private Player ActivePlayer(UserId user)
  {
    CheckIsMember(user);
    var session = CheckStarted();
    var seat = Data.SeatOf(user);
    return session.ActivePlayer(seat);
  }

  private bool IsStarted => Session != null;

  private void CheckNotStarted()
  {
    if (IsStarted)
    {
      throw ErrorCodes.TableStarted.ToException();
    }
  }

  private ISession CheckStarted()
  {
    if (Session is { } session)
    {
      return session;
    }
    throw ErrorCodes.TableNotStarted.ToException();
  }

  private void CheckIsMember(UserId userId)
  {
    if (!Data.Members.Contains(userId))
    {
      throw ErrorCodes.NotMember.ToException();
    }
  }
}
