using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Contracts;

public interface IDeclarableContract : IContract
{
  bool IsAllowed(IEnumerable<Card> playerCards);
  string Id { get; }
}
