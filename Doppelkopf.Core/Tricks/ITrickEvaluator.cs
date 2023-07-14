using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Tricks;

public interface ITrickEvaluator
{
  bool TakesTrickFrom(Card current, Card bestSoFar, bool lastTrick);
  Player GetWinner(ITrick trick, bool isLastTrick);
}

public interface ITrickSuitProvider
{
  TrickSuit GetTrickSuit(Card card);
}
