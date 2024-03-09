using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Scoring;

public static class TrickExtensions
{
  public static int Points(this CompleteTrick trick) => trick.Cards.Values.Select(c => c.Points()).Sum();
}
