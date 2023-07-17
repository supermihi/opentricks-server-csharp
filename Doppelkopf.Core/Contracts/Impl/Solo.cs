using System.Collections.Immutable;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

public class Solo : IDeclarableContract
{
  public Solo(ICardTraitsProvider cardTraits, string id)
  {
    CardTraits = cardTraits;
    Id = id;
  }

  public static Solo MeatFree =>
      new(
        CardTraitsProvider.ForTrumpWithDefaultSides(
          Enumerable.Empty<Card>()), "meat_free");

  public static Solo JackSolo => new(CardTraitsProvider.ForTrumpWithDefaultSides(Card.Jacks), "jack_solo");
  public static Solo QueenSolo => new(CardTraitsProvider.ForTrumpWithDefaultSides(Card.Queens), "queen_solo");

  public static Solo SuitSolo(Suit trump, TieBreakingMode heartTenTieBreaking) =>
      new(CardTraitsProvider.SuitSolo(trump, heartTenTieBreaking), $"{trump.ToString().ToLowerInvariant()}_solo");

  public ICardTraitsProvider CardTraits { get; }
  public string Id { get; }

  public IPartyProvider CreatePartyProvider(Player? declarer, ICardsByPlayer initialCards)
  {
    return PartyProvider.Solo(declarer ?? throw new ArgumentNullException(nameof(declarer)));
  }

  public IReadOnlyList<IExtraPointRule> ExtraPointRules => ImmutableArray<IExtraPointRule>.Empty;

  public ContractType Type => ContractType.Solo;
  public bool IsAllowed(IEnumerable<Card> playerCards) => true;
}
