using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

internal record Solo(string Id, ICardTraitsProvider Traits) : IHold
{
  public bool IsSolo => true;
  public bool IsAllowed(IEnumerable<Card> playerCards) => true;

  public DeclarationPriority Priority { get; } = new(
    DeclarationPriority.Solo,
    DeclarationPriority.CompulsorySolo);

  public IContract CreateContract(Player declarer) => new SoloContract(Traits, declarer);

  public static readonly Solo Fleshless = new(
    HoldIds.FleshlessSolo,
    CardTraitsProvider.ForTrumpWithDefaultSides([]));

  public static readonly Solo JackSolo = new(
    HoldIds.JackSolo,
    CardTraitsProvider.ForTrumpWithDefaultSides(CardUtils.Jacks));

  public static Solo QueenSolo => new(HoldIds.QueenSolo, CardTraitsProvider.ForTrumpWithDefaultSides(CardUtils.Queens));

  public static Solo SuitSolo(Suit trump, TieBreakingMode heartTenTieBreaking) =>
    new(
      HoldIds.SuitSolo(trump),
      CardTraitsProvider.SuitSolo(trump, heartTenTieBreaking)
    );
}
