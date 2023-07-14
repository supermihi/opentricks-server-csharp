namespace Doppelkopf.Core.Tricks.Impl;

internal static class TrickExtensions
{
  public static Trick SetWinner(this Trick trick,
    ITrickEvaluator evaluator,
    bool isLastTrick)
  {
    var winner = evaluator.GetWinner(trick, isLastTrick);
    return trick with { Winner = winner };
  }
}
