using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Contracts;

public interface IHold
{
  string Id { get; }
  bool IsAllowed(IEnumerable<Card> playerCards);
  bool IsSolo { get; }
  DeclarationPriority Priority { get; }

  IContract CreateContract(Player declarer, CardsByPlayer initialCards);
}
