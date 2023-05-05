using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Errors;

namespace Doppelkopf.Tricks;

public sealed record Trick(Player Leader, IImmutableList<Card> Cards)
{
  public static Trick Initial(Player leader) => new(leader, ImmutableArray<Card>.Empty);

  public Player? Turn => IsFull ? null : Leader.Skip(Cards.Count);
  public bool IsFull => Cards.Count == Constants.NumberOfPlayers;

  public bool IsValidNextCard(Card card, ITrickRules rules, IEnumerable<Card> cardsOfPlayer)
  {
    if (IsFull)
    {
      return false;
    }
    if (Cards.Count == 0)
    {
      return true;
    }
    if (rules.SameTrickSuit(Cards[0], card))
    {
      return true;
    }
    var hasSameTrickSuit = cardsOfPlayer.Any(c => rules.SameTrickSuit(c, card));
    return !hasSameTrickSuit;
  }

  public Trick Add(Card card)
  {
    if (IsFull)
    {
      throw new IllegalStateException("can not add card to a full trick");
    }
    return this with { Cards = Cards.Add(card) };
  }

  private Player Winner(TrickContext context)
  {
    if (!IsFull)
    {
      throw new IllegalStateException("can only determine winner of full trick");
    }

    var indexOfWinner = Enumerable
      .Range(0, Cards.Count)
      .Aggregate((best, next) => context.TakesTrickFrom(Cards[next], Cards[best]) ? next : best);
    return Leader.Skip(indexOfWinner);
  }

  public FinishedTrick Finish(TrickContext context)
  {
    var winner = Winner(context);
    return new(this, winner);
  }
}
