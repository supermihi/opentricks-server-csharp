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

  public IContract CreateContract(Player declarer, ICardsByPlayer initialCards) =>
    new SoloContract(Traits, declarer, Id);

  public static readonly Solo Fleshless = new(
    HoldIds.FleshlessSolo,
    CardTraitsProvider.ForTrumpWithDefaultSides(Enumerable.Empty<Card>()));

  public static readonly Solo JackSolo = new(
    HoldIds.JackSolo,
    CardTraitsProvider.ForTrumpWithDefaultSides(Card.Jacks));

  public static Solo QueenSolo => new(HoldIds.QueenSolo, CardTraitsProvider.ForTrumpWithDefaultSides(Card.Queens));

  public static Solo SuitSolo(Suit trump, TieBreakingMode heartTenTieBreaking) =>
    new(
      HoldIds.SuitSolo(trump),
      CardTraitsProvider.SuitSolo(trump, heartTenTieBreaking)
    );
}
