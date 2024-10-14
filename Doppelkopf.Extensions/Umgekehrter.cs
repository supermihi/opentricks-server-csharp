using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Extensions;

public class UmgekehrterHold : IHold
{
  public const string UmgekehrterId = "umgekehrter";
  public string Id => UmgekehrterId;

  public bool IsSolo => true;

  public DeclarationPriority Priority { get; } = new(DeclarationPriority.Solo, DeclarationPriority.CompulsorySolo);

  public IContract CreateContract(Player declarer) => new UmgekehrterContract(declarer);
  public bool IsAllowed(IEnumerable<Card> playerCards) => true;
}

public class UmgekehrterTraits : ICardTraitsProvider
{
  public CardTraits Get(Card card)
  {
    return new CardTraits(card.Suit.AsTrickSuit(), -card.Rank.DefaultSideSuitRank(), TieBreakingMode.FirstWins);
  }
}
public class UmgekehrterContract(Player soloist) : IContract
{
  public ICardTraitsProvider Traits { get; } = new UmgekehrterTraits();

  public IPartyProvider Parties { get; } = IPartyProvider.Solo(soloist);

  public void OnTrickFinished(CompleteTrick trick) { }
}
