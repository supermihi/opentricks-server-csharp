using Doppelkopf.Cards;

namespace Doppelkopf.Server;

public interface ITable {
  TableMeta Meta { get; }
  Task PlayCard(UserId user, Card card);
  Task Reserve(UserId user, bool reserved);
}