using Doppelkopf.Core.Cards;

namespace Doppelkopf.Service;

public sealed record TableEvent
{
  public ITableState Table { get; }
  public UserId Actor { get; }
  public TableEventType EventType { get; }

  public enum TableEventType
  {
    Start,
    PlayCard
  }

  private TableEvent(ITableState table, UserId actor, TableEventType eventType)
  {
    Table = table;
    Actor = actor;
    EventType = eventType;
  }
  public sealed record PlayCardEvent(Card Card, PlayCardResult Result);
  public PlayCardEvent? CardPlayed { get; private init; }

  public static TableEvent Start(ITableState table) => new(table, table.Data.Owner, TableEventType.Start);

  public static TableEvent PlayCard(ITableState table, Card card, UserId player, PlayCardResult result) =>
      new(
        table,
        player,
        TableEventType.PlayCard)
      { CardPlayed = new(card, result) };
}
