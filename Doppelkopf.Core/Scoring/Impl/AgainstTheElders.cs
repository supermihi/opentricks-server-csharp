using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public class AgainstTheElders : IExtraScoreRule
{
  public IEnumerable<Score> Evaluate(IReadOnlyList<CompleteTrick> tricks, ByPlayer<Party> parties,
    Party? winnerOfGame)
  {
    if (winnerOfGame == Party.Contra && parties.Soloist() is null)
    {
      yield return new Score(ScoreIds.AgainstTheElders, Party.Contra);
    }
  }
}
