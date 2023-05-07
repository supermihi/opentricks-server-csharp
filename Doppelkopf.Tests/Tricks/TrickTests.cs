using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.Tricks;
using Moq;

namespace Doppelkopf.Tests.Tricks;

public class TrickTests {
  [Fact]
  public void Initial() {
    var trick = new Trick(Player.Player3);

    Assert.Equal(Player.Player3, trick.Leader);
    Assert.Empty(trick.Cards);
    Assert.False(trick.IsFull);
    Assert.Equal(Player.Player3, trick.Turn);
  }

  [Fact]
  public void Add() {
    var trickWithTwoCards = new Trick(Player.Player2)
                            .AddCard(new Card(Suit.Diamonds, Rank.Queen))
                            .AddCard(new Card(Suit.Clubs, Rank.Nine));
    Assert.Equal(2, trickWithTwoCards.Cards.Count);
    Assert.Equal(new Card(Suit.Diamonds, Rank.Queen), trickWithTwoCards.Cards[0]);
    Assert.Equal(new Card(Suit.Clubs, Rank.Nine), trickWithTwoCards.Cards[1]);

    Assert.False(trickWithTwoCards.IsFull);
    Assert.Equal(Player.Player4, trickWithTwoCards.Turn);
  }

  [Fact]
  public void FullWithFourCards() {
    var trick = new Trick(Player.Player4)
                .AddCard(new(Suit.Hearts, Rank.Ace))
                .AddCard(new(Suit.Hearts, Rank.Ace))
                .AddCard(new(Suit.Hearts, Rank.King))
                .AddCard(new(Suit.Hearts, Rank.Nine));

    Assert.True(trick.IsFull);
    Assert.Null(trick.Turn);
  }

  [Fact]
  public void AddingFifthCardThrows() {
    var fullTrick = new Trick(Player.Player2)
                    .AddCard(new(Suit.Hearts, Rank.Ace))
                    .AddCard(new(Suit.Hearts, Rank.Ace))
                    .AddCard(new(Suit.Hearts, Rank.King))
                    .AddCard(new(Suit.Hearts, Rank.Nine));

    Assert.Throws<IllegalStateException>(() => fullTrick.AddCard(new Card(Suit.Clubs, Rank.Ace)));
  }

  [Fact]
  public void SideSuitAceWinsInNormalGame() {
    var trick = new Trick(Player.Player1)
                .AddCard(new(Suit.Spades, Rank.King))
                .AddCard(new(Suit.Spades, Rank.Ace))
                .AddCard(new(Suit.Hearts, Rank.Nine))
                .AddCard(new(Suit.Spades, Rank.Ace));
    var mode = new NormalGameMode(EldersMode.FirstWins);
    var context = new TrickContext(mode.TrickRules, false);
    var winner = trick.Winner(context);
    Assert.Equal(Player.Player2, winner);
  }

  [Theory]
  [InlineData(EldersMode.FirstWins, false)]
  [InlineData(EldersMode.FirstWins, true)]
  [InlineData(EldersMode.SecondWins, false)]
  [InlineData(EldersMode.SecondWins, true)]
  [InlineData(EldersMode.FirstWinsExceptInLastTrick, false)]
  [InlineData(EldersMode.FirstWinsExceptInLastTrick, true)]
  public void EldersModeIsRespectedInNormalGame(EldersMode elders, bool isLastTrick) {
    var trick = new Trick(Player.Player1)
                .AddCard(new(Suit.Spades, Rank.King))
                .AddCard(new(Suit.Hearts, Rank.Ten))
                .AddCard(new(Suit.Hearts, Rank.Ten))
                .AddCard(new(Suit.Spades, Rank.Ace));
    var mode = new NormalGameMode(elders);
    var context = new TrickContext(mode.TrickRules, isLastTrick);
    var winner = trick.Winner(context);
    var expectedWniner =
        elders == EldersMode.SecondWins
     || (elders == EldersMode.FirstWinsExceptInLastTrick && isLastTrick)
            ? Player.Player3
            : Player.Player2;
    Assert.Equal(expectedWniner, winner);
  }

  [Fact]
  public void IsValidNextCardTrueForEmptyTrick() {
    var trick = new Trick(Player.Player3);
    var trickRules = Mock.Of<ITrickRules>(MockBehavior.Strict);
    var result = trick.IsValidNextCard(new(Suit.Clubs, Rank.Ace), trickRules, Decks.WithNines);

    Assert.True(result);
  }

  [Fact]
  public void IsValidNextCardFalseForFullTrick() {
    var trick = new Trick(Player.Player1).AddCard(new(Suit.Clubs, Rank.Ace))
                                         .AddCard(new(Suit.Clubs, Rank.Ten))
                                         .AddCard(new(Suit.Clubs, Rank.King))
                                         .AddCard(new(Suit.Clubs, Rank.Nine));
    Assert.False(trick.IsValidNextCard(new(Suit.Diamonds, Rank.Ace), Mock.Of<ITrickRules>(MockBehavior.Strict),
                                       Enumerable.Empty<Card>()));
  }

  [Fact]
  public void IsValidNextCardAllowsOtherSuitIfCannotMatch() {
    var trick = new Trick(Player.Player1).AddCard(new(Suit.Spades, Rank.Ace));
    var rules = TrickRules.ForTrumpSuit(Suit.Diamonds, EldersMode.FirstWins);
    var handCards = ImmutableArray.Create<Card>(new(Suit.Spades, Rank.Jack),
                                                new(Suit.Hearts, Rank.Ace));
    Assert.True(trick.IsValidNextCard(handCards[0], rules, handCards));
  }
}