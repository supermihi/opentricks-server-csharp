using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Contracts.Impl;

internal record SoloContract : IContract
{
  private readonly IEvaluator _evaluator;

  public SoloContract(ICardTraitsProvider traits, Player soloist, string id, IEvaluator evaluator)
  {
    _evaluator = evaluator;
    Traits = traits;
    Id = id;
    Parties = new StaticPartyProvider(ByPlayer.Init(p => p == soloist ? Party.Re : Party.Contra));
  }

  public IPartyProvider Parties { get; }

  public GameEvaluation Evaluate(IReadOnlyList<CompleteTrick> tricks, ByParty<Bid?> maxBids) =>
    _evaluator.Evaluate(tricks, maxBids, Parties.GetAll());

  public ICardTraitsProvider Traits { get; }
  public string Id { get; }

  public void OnTrickFinished(CompleteTrick trick)
  {
  }
}
