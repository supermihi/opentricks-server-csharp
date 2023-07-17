using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Scoring;

public static class TrickExtensions
{
  public static int Score(this CompleteTrick trick) => trick.Cards.Select(c => c.Points()).Sum();
}
