using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

internal record Wedding(TieBreakingMode HeartTenTieBreaking) : IHold
{
  public bool IsAllowed(IEnumerable<Card> playerCards) => playerCards.Count(c => c == Card.ClubsQueen) == 2;

  public bool IsSolo => false;
  public string Id => HoldIds.Wedding;
  public DeclarationPriority Priority { get; } = new(DeclarationPriority.Wedding);

  public IContract CreateContract(Player declarer) => new WeddingContract(HeartTenTieBreaking, declarer, true);
}
