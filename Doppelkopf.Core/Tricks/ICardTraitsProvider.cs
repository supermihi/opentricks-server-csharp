using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Tricks;

public interface ICardTraitsProvider
{
  CardTraits GetTraits(Card card);
  bool TakesTrickFrom(Card current, Card bestSoFar, bool lastTrick);
}
