using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.Tricks;

namespace Doppelkopf.Tests.Tricks;

public class TrickTests
{
  [Fact]
  public void Initial()
  {
    var trick = Trick.Initial(Player.Player3);

    Assert.Equal(Player.Player3, trick.Leader);
    Assert.Empty(trick.Cards);
    Assert.False(trick.IsFull);
    Assert.Equal(Player.Player3, trick.Turn);
  }

  [Fact]
  public void Add()
  {
    var trickWithTwoCards = Trick
      .Initial(Player.Player2)
      .Add(new Card(Suit.Diamonds, Rank.Queen))
      .Add(new Card(Suit.Clubs, Rank.Nine));
    Assert.Equal(2, trickWithTwoCards.Cards.Count);
    Assert.Equal(new Card(Suit.Diamonds, Rank.Queen), trickWithTwoCards.Cards[0]);
    Assert.Equal(new Card(Suit.Clubs, Rank.Nine), trickWithTwoCards.Cards[1]);

    Assert.False(trickWithTwoCards.IsFull);
    Assert.Equal(Player.Player4, trickWithTwoCards.Turn);
  }

  [Fact]
  public void FullWithFourCards()
  {
    var trick = new Trick(
      Player.Player4,
      ImmutableArray.Create(
        new Card(Suit.Hearts, Rank.Ace),
        new Card(Suit.Hearts, Rank.Ace),
        new Card(Suit.Hearts, Rank.King),
        new Card(Suit.Hearts, Rank.Nine)
      )
    );
    Assert.True(trick.IsFull);
    Assert.Null(trick.Turn);
  }

  [Fact]
  public void AddingFifthCardThrows()
  {
    var fullTrick = new Trick(
      Player.Player2,
      ImmutableArray.Create(
        new Card(Suit.Hearts, Rank.Ace),
        new Card(Suit.Hearts, Rank.Ace),
        new Card(Suit.Hearts, Rank.King),
        new Card(Suit.Hearts, Rank.Nine)
      )
    );

    Assert.Throws<IllegalStateException>(() => fullTrick.Add(new Card(Suit.Clubs, Rank.Ace)));
  }

  [Fact]
  public void SideSuitAceWinsInNormalGame()
  {
    var trick = new Trick(
      Player.Player1,
      ImmutableArray.Create<Card>(
        new Card(Suit.Spades, Rank.King),
        new Card(Suit.Spades, Rank.Ace),
        new Card(Suit.Hearts, Rank.Nine),
        new Card(Suit.Spades, Rank.Ace)
      )
    );
    var mode = new NormalGameMode();
    var context = new TrickContext(mode.TrickRules, EldersMode.FirstWins, false);
    var finishedTrick = trick.Finish(context);
    Assert.Equal(finishedTrick.Trick, trick);
    Assert.Equal(Player.Player2, finishedTrick.Winner);
  }
}
