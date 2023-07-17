using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

public class Marriage : IDeclarableContract
{
  public bool IsAllowed(IEnumerable<Card> playerCards) => playerCards.Count(c => c == Card.ClubsQueen) == 2;

  public string Id => "marriage";
  public Marriage(TieBreakingMode heartsTenTieBreaking)
  {
    CardTraits = CardTraitsProvider.SuitSolo(Suit.Diamonds, heartsTenTieBreaking);
  }

  public ICardTraitsProvider CardTraits { get; }
  public IReadOnlyList<IExtraPointRule> ExtraPointRules => ExtraPoints.Default;

  public IPartyProvider CreatePartyProvider(Player? declarer, ICardsByPlayer initialCards) =>
      new MarriagePartyProvider(
        declarer ?? throw new ArgumentNullException(nameof(declarer)));

  public ContractType Type => ContractType.Marriage;
  public bool? IsCompulsorySolo => null;
}
