using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Conf;
using Doppelkopf.Contracts;
using Doppelkopf.Errors;
using Doppelkopf.Tricks;

namespace Doppelkopf.Tests.Tricks;

public class TrickTests
{
    [Fact]
    public void Initial()
    {
        var trick = new Trick(Player.Player3);

        Assert.Equal(Player.Player3, trick.Leader);
        Assert.Empty(trick.Cards);
        Assert.False(trick.IsFull);
        Assert.Equal(Player.Player3, trick.Turn);
    }

    [Fact]
    public void Add()
    {
        var trickWithTwoCards = new Trick(Player.Player2).AddCard(new Card(Suit.Diamonds, Rank.Queen))
                .AddCard(new Card(Suit.Clubs, Rank.Nine));
        Assert.Equal(2, trickWithTwoCards.Cards.Count);
        Assert.Equal(new Card(Suit.Diamonds, Rank.Queen), trickWithTwoCards.Cards[0]);
        Assert.Equal(new Card(Suit.Clubs, Rank.Nine), trickWithTwoCards.Cards[1]);

        Assert.False(trickWithTwoCards.IsFull);
        Assert.Equal(Player.Player4, trickWithTwoCards.Turn);
    }

    [Fact]
    public void FullWithFourCards()
    {
        var trick = new Trick(Player.Player4)
                .AddCard(new(Suit.Hearts, Rank.Ace))
                .AddCard(new(Suit.Hearts, Rank.Ace))
                .AddCard(new(Suit.Hearts, Rank.King))
                .AddCard(new(Suit.Hearts, Rank.Nine));

        Assert.True(trick.IsFull);
        Assert.Null(trick.Turn);
    }

    [Fact]
    public void AddingFifthCardThrows()
    {
        var fullTrick = new Trick(Player.Player2)
                .AddCard(new(Suit.Hearts, Rank.Ace))
                .AddCard(new(Suit.Hearts, Rank.Ace))
                .AddCard(new(Suit.Hearts, Rank.King))
                .AddCard(new(Suit.Hearts, Rank.Nine));

        Assert.Throws<IllegalStateException>(() => fullTrick.AddCard(new Card(Suit.Clubs, Rank.Ace)));
    }

    [Fact]
    public void SideSuitAceWinsInNormalGame()
    {
        var trick = new Trick(Player.Player1)
                .AddCard(new(Suit.Spades, Rank.King))
                .AddCard(new(Suit.Spades, Rank.Ace))
                .AddCard(new(Suit.Hearts, Rank.Nine))
                .AddCard(new(Suit.Spades, Rank.Ace));
        var mode = new NormalGame();
        var context = new TrickContext(mode.CardTraits, new(EldersMode.FirstWins), false);
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
    public void EldersModeIsRespectedInNormalGame(EldersMode elders, bool isLastTrick)
    {
        var trick = new Trick(Player.Player1)
                .AddCard(new(Suit.Spades, Rank.King))
                .AddCard(new(Suit.Hearts, Rank.Ten))
                .AddCard(new(Suit.Hearts, Rank.Ten))
                .AddCard(new(Suit.Spades, Rank.Ace));
        var context = new TrickContext(new NormalGame().CardTraits, new(elders), isLastTrick);
        var winner = trick.Winner(context);
        var expectedWinner =
                elders == EldersMode.SecondWins || (elders == EldersMode.FirstWinsExceptInLastTrick && isLastTrick)
                        ? Player.Player3
                        : Player.Player2;
        Assert.Equal(expectedWinner, winner);
    }

    [Fact]
    public void EldersModeIgnoredForMeatfree()
    {
        var trick = new Trick(Player.Player3).AddCard(new(Suit.Hearts, Rank.Nine))
                .AddCard(new(Suit.Hearts, Rank.Ten))
                .AddCard(new(Suit.Hearts, Rank.Ten))
                .AddCard(new(Suit.Spades, Rank.Ace));
        var context = new TrickContext(CardTraitsProvider.NoTrump, new(EldersMode.SecondWins), false);
        var winner = trick.Winner(context);
        Assert.Equal(Player.Player4, winner);
    }

    [Fact]
    public void IsValidNextCardTrueForEmptyTrick()
    {
        var trick = new Trick(Player.Player3);
        var result = trick.IsValidNextCard(
            new(Suit.Clubs, Rank.Ace),
            Decks.WithNines,
            CardTraitsProvider.DiamondsTrump);

        Assert.True(result);
    }

    [Fact]
    public void IsValidNextCardFalseForFullTrick()
    {
        var trick = new Trick(Player.Player1).AddCard(new(Suit.Clubs, Rank.Ace))
                .AddCard(new(Suit.Clubs, Rank.Ten))
                .AddCard(new(Suit.Clubs, Rank.King))
                .AddCard(new(Suit.Clubs, Rank.Nine));
        Assert.False(
            trick.IsValidNextCard(
                new(Suit.Diamonds, Rank.Ace),
                Enumerable.Empty<Card>(),
                CardTraitsProvider.DiamondsTrump));
    }

    [Fact]
    public void IsValidNextCardAllowsOtherSuitIfCannotMatch()
    {
        var trick = new Trick(Player.Player1).AddCard(new(Suit.Spades, Rank.Ace));
        var contract = CardTraitsProvider.DiamondsTrump;
        var handCards = ImmutableArray.Create<Card>(new(Suit.Spades, Rank.Jack), new(Suit.Hearts, Rank.Ace));
        Assert.True(trick.IsValidNextCard(handCards[0], handCards, contract));
    }
}
