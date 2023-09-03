using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Games;
using Doppelkopf.Users.API;

namespace Doppelkopf.Service;

public interface ITable : ITableState
{
  Task Start();
  Task<PlayCardResult> PlayCard(UserId user, Card card);
}
