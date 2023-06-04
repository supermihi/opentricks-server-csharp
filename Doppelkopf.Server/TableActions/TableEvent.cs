using Doppelkopf.API;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.TableActions;

public record TableEvent(EventType Type, UserId? Actor)
{
  /// <summary>
  /// Id of the user that initiated the action. May be <c>null</c> in the user's request (where the actor is identified
  /// by authentication), and is <c>null</c> if the game itself is the "actor".
  /// </summary>
  public UserId? Actor { get; init; } = Actor;

  public static TableEvent CreateTable(Table table) => new(EventType.TableCreated, table.Meta.Owner);

  public static TableEvent JoinTable(UserId joiner) => new(EventType.JoinedTable, joiner);
  public static TableEvent MarkReady(UserId user) => new(EventType.MarkedReady, user);
  public static TableEvent Start() => new(EventType.Started, null);

  public static TableEvent Reserve(UserId user, bool reserved) => new(EventType.Reserved, user) { Reserved = reserved };

  public static TableEvent Declare(UserId user, ContractType contractType) =>
      new(EventType.Declared, user) { ContractType = contractType };

  /// <summary>
  /// Type of the declared contract, for <see cref="EventType.Declared"/> actions.
  /// </summary>
  public ContractType? ContractType { get; private init; }

  public static TableEvent PlayCard(UserId user, Card card) => new(EventType.CardPlayed, user) { Card = card };
  public static TableEvent FinishTrick(UserId winner) => new(EventType.TrickFinished, winner);

  /// <summary>
  /// Id of the played card. Only set if <see cref="Type"/> is <see cref="EventType.CardPlayed"/>.
  /// </summary>
  public Card? Card { get; private init; }

  /// <summary>
  /// Details of the contract. Only set if <see cref="Type"/> is <see cref="EventType.Contracted"/>.
  /// </summary>
  public ContractedData? Contract { get; private init; }

  /// <summary>
  /// Reservation status. Only set if <see cref="Type"/> is <see cref="EventType.Reserved"/>.
  /// </summary>
  public bool? Reserved { get; private init; }

  public static TableEvent Contracted(string contractId, UserId? declarer)
  {
    return new(EventType.Contracted, null) { Contract = new(contractId, declarer) };
  }

  public static TableEvent FinishGame(bool sessionFinished)
  {
    return new(EventType.GameFinished, null) { SessionFinished = sessionFinished };
  }

  public bool? SessionFinished { get; private init; }
}

public record ContractedData(string ContractId, UserId? Soloist);
