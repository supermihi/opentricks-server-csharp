using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core;

public interface ICardsByPlayer
{
  IReadOnlyCollection<Card> GetCards(Player p);
}
