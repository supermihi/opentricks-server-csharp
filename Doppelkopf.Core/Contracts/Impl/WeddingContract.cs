using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

/// <summary>
/// Normal game contract, including all kinds of wedding (announced and silent).
/// </summary>
internal class WeddingContract(
  TieBreakingMode heartsTenTieBreaking,
  Player suitor,
  bool isAnnouncedWedding,
  IEvaluator evaluator)
  : IContract, IPartyProvider
{
  public ICardTraitsProvider Traits { get; } = CardTraitsProvider.NormalGame(heartsTenTieBreaking);
  private Player? _spouse;

  public Party? Get(Player player)
  {
    if (player == suitor || player == _spouse)
    {
      return Party.Re;
    }

    return DefiningTrick == null ? null : Party.Contra;
  }

  public IPartyProvider Parties => this;

  public GameEvaluation Evaluate(IReadOnlyList<CompleteTrick> tricks, ByParty<Bid?> maxBids) =>
    evaluator.Evaluate(tricks, maxBids, Parties.GetAll());

  public int? DefiningTrick { get; private set; } = isAnnouncedWedding ? null : 0;

  public void OnTrickFinished(CompleteTrick trick)
  {
    if (DefiningTrick != null)
    {
      return;
    }

    const int maxTrickIndexToFindSpouse = 2;
    if (trick.Winner != suitor)
    {
      DefiningTrick = trick.Index;
      _spouse = trick.Winner;
    }

    if (trick.Index != maxTrickIndexToFindSpouse)
    {
      return;
    }

    DefiningTrick = trick.Index;
  }
}
