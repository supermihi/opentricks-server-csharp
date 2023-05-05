using Doppelkopf.Cards;
using Doppelkopf.Configuration;

namespace Doppelkopf.Tricks;

public sealed record TrickContext(ITrickRules Rules, EldersMode Elders, bool IsLastTrick)
{
  public bool TakesTrickFrom(Card next, Card bestSoFar)
  {
    var comparison = Rules.Compare(next, bestSoFar);

    return comparison switch
    {
      CardComparison.Higher => true,
      CardComparison.Lower => false,
      CardComparison.Equal when next == Card.TenOfHearts || bestSoFar == Card.TenOfHearts
        => TakesTrickFromTwoElders,
      CardComparison.Equal => false,
      _ => throw new ArgumentOutOfRangeException()
    };
  }

  private bool TakesTrickFromTwoElders =>
    Elders switch
    {
      EldersMode.FirstWins => false,
      EldersMode.SecondWins => true,
      EldersMode.FirstWinsExceptInLastTrick => IsLastTrick,
      _ => throw new ArgumentOutOfRangeException()
    };
}
