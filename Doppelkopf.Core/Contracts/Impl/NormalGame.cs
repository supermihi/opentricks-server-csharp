using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

public class NormalGame : IContract
{
  public NormalGame(TieBreakingMode heartsTenTieBreaking)
  {
    CardTraits = CardTraitsProvider.SuitSolo(Suit.Diamonds, heartsTenTieBreaking);
  }

  public ICardTraitsProvider CardTraits { get; }
  public IReadOnlyList<IExtraPointRule> ExtraPointRules => ExtraPoints.Default;

  public IPartyProvider CreatePartyProvider(Player? declarer, ICardsByPlayer initialCards)
  {
    return new PartyProvider(
      Enum.GetValues<Player>()
          .Where(p => initialCards.GetCards(p).Any(card => card == Card.ClubsQueen))
          .ToArray());
  }

  public ContractType Type => ContractType.NormalGame;
}
