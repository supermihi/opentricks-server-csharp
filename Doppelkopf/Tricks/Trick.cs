using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Errors;

namespace Doppelkopf.Tricks;

public sealed record Trick
{
  private readonly InTurns<Card> _data;

  public Trick(Player leader)
    : this(new InTurns<Card>(leader)) { }

  private Trick(InTurns<Card> data)
  {
    _data = data;
  }

  public bool IsFull => _data.IsFull;
  public Player Leader => _data.First;
  public Player? Turn => _data.Next;
  public IImmutableList<Card> Cards => _data.Elements;

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
    var definingCard = Cards[0];
    if (rules.SameTrickSuit(definingCard, card))
    {
      return true;
    }
    var hasSameTrickSuit = cardsOfPlayer.Any(c => rules.SameTrickSuit(c, definingCard));
    return !hasSameTrickSuit;
  }

  public Trick AddCard(Card card)
  {
    if (IsFull)
    {
      throw new IllegalStateException("can not add card to a full trick");
    }
    return new(_data.Add(card));
  }

  public Player Winner(TrickContext context)
  {
    if (!IsFull)
    {
      throw new IllegalStateException("can only determine winner of full trick");
    }
    var (rules, isLast) = context;
    var indexOfWinner = Enumerable
      .Range(0, Cards.Count)
      .Aggregate(
        (best, next) => rules.TakesTrickFrom(Cards[next], Cards[best], isLast) ? next : best
      );
    return Leader.Skip(indexOfWinner);
  }
}
