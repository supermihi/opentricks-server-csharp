using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts;

public interface IContract
{
  void OnTrickFinished(CompleteTrick trick);
  ICardTraitsProvider Traits { get; }
  IPartyProvider Parties { get; }
  GameEvaluation Evaluate(IReadOnlyList<CompleteTrick> tricks, ByParty<Bid?> maxBids);
}
