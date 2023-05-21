using Doppelkopf.Cards;

namespace Doppelkopf.Server.Model;

public interface ITable
{
  TableData Data { get; }
  Task AddUser(UserId user);
  Task PlayCard(UserId user, Card card);
  Task Reserve(UserId user, bool reserved);
}
