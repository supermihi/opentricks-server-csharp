using Doppelkopf.Cards;
using Doppelkopf.Contracts;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.TableActions;

public interface ITableEvent {}

public record CreateTableEvent(Table Table) : ITableEvent;
public record JoinTableEvent(UserId Joiner) : ITableEvent;
public record MarkedReadyEvent(UserId User) : ITableEvent;
public record StartTableEvent(UserId User) : ITableEvent;
public record ReserveEvent(UserId User, bool IsReserved) : ITableEvent;
public record DeclareEvent(UserId User, ContractType ContractType) : ITableEvent;
public record PlayCardEvent(UserId User, Card Card) : ITableEvent;
public record FinishTrickEvent(UserId Winner) : ITableEvent;
public record AuctionFinishedEvent : ITableEvent;
public record GameFinishedEvent(bool SessionFinished) : ITableEvent;

