using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring;

public interface IEvaluator
{
  GameEvaluation Evaluate(IReadOnlyList<CompleteTrick> tricks, ByParty<Bid?> maxBids, ByPlayer<Party> parties);
}
