using Doppelkopf.Cards;

namespace Doppelkopf.Contracts;

public interface ICardTraitsProvider
{
  CardTraits Get(Card card);
}
