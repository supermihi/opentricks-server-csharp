using Doppelkopf.API;
using Doppelkopf.Server.Interface;
using Doppelkopf.Server.TableActions;
using EventType = Doppelkopf.API.EventType;

namespace Doppelkopf.Server.Notifications;

public static class NotificationExtensions
{
  public static Notification ToNotification(this TableActionResult result, UserId maskFor)
  {
    return new(
      result.Table.ToTableState(maskFor),
      result.Events.Select(ev => ev.ToApiEvent()).ToList(),
      result.Timestamp);
  }

  public static Event ToApiEvent(this ITableEvent tableEvent)
  {
    switch (tableEvent)
    {
      case CreateTableEvent create: return new Event(EventType.TableCreated);
      case MarkedReadyEvent ready: return new(EventType.MarkedReady, ready.User);
      case JoinTableEvent join: return new(EventType.JoinedTable, join.Joiner);
      case StartTableEvent start: return new Event(EventType.Started, start.User);
      case ReserveEvent reserve: return new Event(EventType.Reserved, reserve.User, Reserved: reserve.IsReserved);
      case DeclareEvent declare: return new Event(EventType.Declared);
      case AuctionFinishedEvent auction: return new Event(EventType.Contracted);
      case PlayCardEvent playCard: return new(EventType.CardPlayed, playCard.User, playCard.Card.Id);
      default: throw new NotImplementedException();
    }
  }
}
