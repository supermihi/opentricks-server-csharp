using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring;

public interface IExtraScoreRule
{
  IEnumerable<Score> Evaluate(IReadOnlyList<CompleteTrick> tricks, ByPlayer<Party> parties, Party? winnerOfGame);
}
