using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public class AgainstTheElders : IExtraPointRule
{
  public IEnumerable<ExtraPoint> Evaluate(IReadOnlyList<CompleteTrick> tricks, ByPlayer<Party> parties,
    Party? winnerOfGame)
  {
    if (winnerOfGame == Party.Contra && parties.Soloist() is null)
    {
      yield return new ExtraPoint(ExtraPointIds.AgainstTheElders, null, Party.Contra, null);
    }
  }
}
