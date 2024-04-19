using System.Collections.Immutable;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;
using Xunit;

namespace Doppelkopf.Core.Tests.Tricks;

public class TrickTests
{
  [Fact]
  public void Initial()
  {
    var trick = new Trick(Player.Three, 0, 12);

    Assert.Equal(Player.Three, trick.Leader);
    Assert.Empty(trick.Cards);
    Assert.False(trick.Cards.IsFull);
    Assert.Equal(Player.Three, trick.Cards.Next);
  }

  [Fact]
  public void Add()
  {
    var trickWithTwoCards = new Trick(
      new InTurns<Card>(Player.Two, new Card(Suit.Diamonds, Rank.Queen), new Card(Suit.Clubs, Rank.Nine)),
      0,
      12);
    Assert.Equal(2, trickWithTwoCards.Cards.Count);
    Assert.Equal(new Card(Suit.Diamonds, Rank.Queen), trickWithTwoCards.Cards[0]);
    Assert.Equal(new Card(Suit.Clubs, Rank.Nine), trickWithTwoCards.Cards[1]);

    Assert.False(trickWithTwoCards.Cards.IsFull);
    Assert.Equal(Player.Four, trickWithTwoCards.Cards.Next);
  }

  [Fact]
  public void FullWithFourCards()
  {
    var trick = new Trick(new InTurns<Card>(
      Player.Four,
      new(Suit.Hearts, Rank.Ace),
      new(Suit.Hearts, Rank.Ace),
      new(Suit.Hearts, Rank.King),
      new(Suit.Hearts, Rank.Nine)),
      0,
      12);

    Assert.True(trick.Cards.IsFull);
    Assert.Null(trick.Cards.Next);
  }

  [Fact]
  public void AddingFifthCardThrows()
  {
    var trick = new Trick(new InTurns<Card>(
        Player.Four,
        new(Suit.Hearts, Rank.Ace),
        new(Suit.Hearts, Rank.Ace),
        new(Suit.Hearts, Rank.King),
        new(Suit.Hearts, Rank.Nine)),
      0,
      12);

    Assert.Throws<ArgumentException>(() => trick.AddCard(new Card(Suit.Clubs, Rank.Ace)));
  }
}
