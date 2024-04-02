using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Tricks;

public interface ICardTraitsProvider
{
  CardTraits Get(Card card);
}
