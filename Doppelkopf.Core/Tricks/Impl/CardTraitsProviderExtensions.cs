using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks.Impl;

public static class CardTraitsProviderExtensions
{
  public static Player GetTrickWinner(this ICardTraitsProvider provider, InTurns<Card> trick, bool isLastTrick)
  {
    if (!trick.IsFull)
    {
      throw new InvalidOperationException("can only determine winner of full trick");
    }
    var indexOfWinner = Enumerable
        .Range(0, trick.Count)
        .Aggregate(
          (best, next) => provider.TakesTrickFrom(trick[next], trick[best], isLastTrick)
              ? next
              : best);
    return trick.Start.Skip(indexOfWinner);
  }

  public static CompletedTrick Complete(this ICardTraitsProvider provider, InTurns<Card> trick, bool isLastTrick)
  {
    var winner = provider.GetTrickWinner(trick, isLastTrick);
    return new CompletedTrick(ByPlayer.Init(p => trick[p]), trick.Start, winner);
  }
}
