using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Conf;
using Doppelkopf.Contracts;
using Doppelkopf.Errors;

namespace Doppelkopf.Tricks;

public sealed record Trick
{
  private readonly InTurns<Card> _data;

  public Trick(Player leader)
      : this(new InTurns<Card>(leader))
  { }

  private Trick(InTurns<Card> data)
  {
    _data = data;
  }

  public bool IsFull => _data.IsFull;
  public Player Leader => _data.First;
  public Player? Turn => _data.Next;
  public IImmutableList<Card> Cards => _data.Elements;

  public bool IsValidNextCard(Card card, IEnumerable<Card> cardsOfPlayer, ICardTraitsProvider contract)
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

    return contract.FollowsSuit(definingCard, card)
        || cardsOfPlayer.All(c => !contract.FollowsSuit(c, definingCard));
  }

  public Trick AddCard(Card card)
  {
    if (IsFull)
    {
      throw new IllegalStateException("can not add card to a full trick");
    }
    return new(_data.Add(card));
  }

  private static bool TakesTrickFrom(Card current, Card bestSoFar, TrickContext context)
  {
    var (cardTraits, config, isLastTrick) = context;
    if (current == bestSoFar
        && current == Card.TenOfHearts
        && cardTraits.Get(Card.TenOfHearts).TrickSuit == TrickSuit.Trump)
    {
      return config.EldersMode switch
      {
          EldersMode.FirstWins => false,
          EldersMode.SecondWins => true,
          EldersMode.FirstWinsExceptInLastTrick => isLastTrick,
          _ => throw new ArgumentOutOfRangeException()
      };
    }

    var (currentSuit, currentRank) = cardTraits.Get(current);
    var (bestSuit, bestRank) = cardTraits.Get(bestSoFar);

    return (currentSuit, bestSuit) switch
    {
        (TrickSuit.Trump, not TrickSuit.Trump) => true,
        (not TrickSuit.Trump, TrickSuit.Trump) => false,
        _ => currentSuit == bestSuit && currentRank > bestRank,
    };
  }

  public Player Winner(TrickContext context)
  {
    if (!IsFull)
    {
      throw new IllegalStateException("can only determine winner of full trick");
    }
    var indexOfWinner = Enumerable
        .Range(0, Cards.Count)
        .Aggregate((best, next) => TakesTrickFrom(Cards[next], Cards[best], context) ? next : best);
    return Leader.Skip(indexOfWinner);
  }
}
