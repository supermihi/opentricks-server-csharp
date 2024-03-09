using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Contracts.Impl;

/// <summary>
/// Normal game contract (no wedding).
/// </summary>
internal class NormalGameContract(TieBreakingMode heartsTenTieBreaking, ByPlayer<bool> hasClubQueen) : IContract
{
  private readonly ICardTraitsProvider _cardTraits = CardTraitsProvider.SuitSolo(Suit.Diamonds, heartsTenTieBreaking);
  public CardTraits GetTraits(Card card) => _cardTraits.GetTraits(card);

  public Party? GetParty(Player player) => hasClubQueen[player] ? Party.Re : Party.Contra;
  public int? DefiningTrick => 0;

  public void OnTrickFinished(CompleteTrick trick)
  {
  }
}
