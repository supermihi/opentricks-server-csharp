namespace Doppelkopf.Core.Tricks;

public interface ITrickTakingProgress
{
  IReadOnlyList<CompleteTrick> CompleteTricks { get; }
  Trick? CurrentTrick { get; }
  CardsByPlayer RemainingCards { get; }
}
