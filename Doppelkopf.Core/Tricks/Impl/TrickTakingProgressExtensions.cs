namespace Doppelkopf.Core.Tricks.Impl;

public static class TrickTakingProgressExtensions
{
  public static int NumCardsPlayed(this ITrickTakingProgress trickTakingProgress, Player player)
  {
    return trickTakingProgress.CompleteTricks.Count
        + (trickTakingProgress.CurrentTrick?.Cards.Contains(player) is true ? 1 : 0);
  }
}
