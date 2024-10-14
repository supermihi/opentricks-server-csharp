using Doppelkopf.Core.Cards;

namespace Doppelkopf.Service;

public interface ITable : ITableState
{
  Task Start();
  Task<PlayCardResult> PlayCard(UserId user, Card card);
}
