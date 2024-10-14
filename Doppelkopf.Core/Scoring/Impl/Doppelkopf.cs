using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public class Doppelkopf : IExtraScoreRule
{
  private const int MinScore = 40;

  public IEnumerable<Score>
    Evaluate(IReadOnlyList<CompleteTrick> tricks, ByPlayer<Party> parties, Party? winnerOfGame)
  {
    if (parties.Soloist() is not null)
    {
      return [];
    }
    return tricks.Where(trick => trick.Points() >= MinScore)
      .Select(
        trick => new Score(
          ScoreIds.Doppelkopf,
          parties[trick.Winner],
          1,
          trick.Winner,
          trick.Index));
  }
}
