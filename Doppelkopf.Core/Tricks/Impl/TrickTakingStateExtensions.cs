namespace Doppelkopf.Core.Tricks.Impl;

internal static class TrickTakingStateExtensions
{
  internal static TrickTakingState UpdateCurrentAndStartNextIfNeeded(this TrickTakingState state,
    ITrickEvaluator evaluator)
  {
    var currentTrick = state.Tricks[^1];
    if (!currentTrick.Cards.IsFull || currentTrick.Winner.HasValue)
    {
      return state;
    }
    var isLastTrick = state.CurrentTrickIsLast();
    var completedCurrentTrick = currentTrick.SetWinner(evaluator, isLastTrick);
    var updatedTricks = state.Tricks[..^1].Add(completedCurrentTrick);
    if (isLastTrick)
    {
      return state with { Tricks = updatedTricks };
    }
    var nextTrick = new Trick(completedCurrentTrick.Winner!.Value);
    return state with { Tricks = updatedTricks.Add(nextTrick) };
  }
}
