using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public class Doppelkopf : IExtraPointRule
{
  private const int MinScore = 40;

  public IEnumerable<ExtraPoint>
    Evaluate(IReadOnlyList<CompleteTrick> tricks, ByPlayer<Party> parties, Party? winnerOfGame)
  {
    if (parties.Soloist() is not null)
    {
      return Enumerable.Empty<ExtraPoint>();
    }
    return tricks.Where(trick => trick.Points() >= MinScore)
      .Select(
        trick => new ExtraPoint(
          ExtraPointIds.Doppelkopf,
          trick.Winner,
          parties[trick.Winner],
          trick.Index));
  }
}
