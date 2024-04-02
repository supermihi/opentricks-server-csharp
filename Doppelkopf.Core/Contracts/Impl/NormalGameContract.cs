using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Contracts.Impl;

/// <summary>
/// Normal game contract (no wedding).
/// </summary>
internal class NormalGameContract(
  TieBreakingMode heartsTenTieBreaking,
  IReadOnlyDictionary<Player, bool> hasClubQueen,
  IEvaluator evaluator) : IContract
{
  public ICardTraitsProvider Traits { get; } = CardTraitsProvider.SuitSolo(Suit.Diamonds, heartsTenTieBreaking);

  public IPartyProvider Parties { get; } =
    new StaticPartyProvider(ByPlayer.Init(p => hasClubQueen[p] ? Party.Re : Party.Contra));

  public void OnTrickFinished(CompleteTrick trick)
  {
  }

  public GameEvaluation Evaluate(IReadOnlyList<CompleteTrick> tricks, ByParty<Bid?> maxBids) =>
    evaluator.Evaluate(tricks, maxBids, Parties.GetAll());
}
