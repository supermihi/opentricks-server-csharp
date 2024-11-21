using System.Collections.Immutable;
using Doppelkopf.API.Errors;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Tricks.Impl;
using Doppelkopf.Core.Utils;
using Doppelkopf.TestUtils;
using Moq;
using Xunit;

namespace Doppelkopf.Core.Tests.Tricks;

public class PlayCardTests
{
  [Fact]
  public void InitialState()
  {
    var state = TrickTakingState.Initial(CardFactory.PlayersCards(true, 3847));
    Assert.NotNull(state.CurrentTrick);
    Assert.Empty(state.CompleteTricks);
    Assert.Equal(Player.One, state.CurrentTrick!.Leader);
    Assert.Empty(state.CurrentTrick.Cards);
  }

  [Fact]
  public void PlayInitialCardSuccess()
  {
    var provider = Mock.Of<ICardTraitsProvider>();
    var cards = CardFactory.PlayersCards(true, 1245).Reduce(3);
    var state = TrickTakingState.Initial(cards);
    var trickTaking = new TrickTaking(provider, state);

    var card = cards[Player.One][0];
    trickTaking.PlayCard(Player.One, card);
    Assert.Equal(card, trickTaking.CurrentTrick!.Cards[0]);
    var expected = cards[Player.One][1..];
    Assert.Equal(expected.ToList(), trickTaking.RemainingCards[Player.One]);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void CompletesTrick(bool isLastTrick)
  {
    var traits = new Mock<ICardTraitsProvider>();
    traits.Setup(t => t.Get(It.IsAny<Card>()))
      .Returns(new CardTraits(TrickSuit.Trump, 1, TieBreakingMode.SecondWins));
    var cards = CardFactory.PlayersCards(true, 1234).Reduce(isLastTrick ? 1 : 2);
    var state = TrickTakingState.Initial(cards);

    var trickTaking = new TrickTaking(traits.Object, state);

    Assert.Null(trickTaking.PlayCard(Player.One, trickTaking.RemainingCards[Player.One].First()));
    Assert.Null(trickTaking.PlayCard(Player.Two, trickTaking.RemainingCards[Player.Two].First()));
    Assert.Null(trickTaking.PlayCard(Player.Three, trickTaking.RemainingCards[Player.Three].First()));
    var trick = trickTaking.PlayCard(Player.Four, trickTaking.RemainingCards[Player.Four].First());
    Assert.NotNull(trick);
    Assert.Equal(Player.Four, trick.Winner);
    Assert.Null(trickTaking.CurrentTrick);

    Assert.Equal(!isLastTrick, trickTaking.TryStartNextTrick());
    if (isLastTrick)
    {
      Assert.Null(trickTaking.CurrentTrick);
    }
    else
    {
      Assert.Equal(Player.Four, trickTaking.CurrentTrick!.Leader);
      Assert.Empty(trickTaking.CurrentTrick.Cards);
    }
  }


  [Theory]
  [InlineData(Player.Two)]
  [InlineData(Player.Three)]
  [InlineData(Player.Four)]
  public void ErrorWhenNotPlayersTurn(Player player)
  {
    var trickTaking = new TrickTaking(Mock.Of<ICardTraitsProvider>(), CardFactory.PlayersCards(true, 4783));
    Assert.Equal(Player.One, trickTaking.CurrentTrick!.Cards.Next);
    Asserts.ThrowsErrorCode(
      ErrorCodes.NotYourTurn,
      () => trickTaking.PlayCard(player, trickTaking.RemainingCards[player].First()));
  }

  [Fact]
  public void ErrorWhenDoesNotHaveCard()
  {
    var trickTaking = new TrickTaking(Mock.Of<ICardTraitsProvider>(), CardFactory.PlayersCards(true, 4783));
    var cardNotInPlayersHand = Decks.WithNines.First(c => !trickTaking.RemainingCards[Player.One].Contains(c));
    Asserts.ThrowsErrorCode(
      ErrorCodes.CardNotOwned,
      () => trickTaking.PlayCard(Player.One, cardNotInPlayersHand)
    );
  }

  [Fact]
  public void ErrorWhenCardNotValid()
  {
    var trickTaking = new TrickTaking(
      CardTraitsProvider.NormalGame(TieBreakingMode.FirstWins),
      new ByPlayer<ImmutableArray<Card>>(
        [
          new Card(Suit.Spades, Rank.Ace),
          new Card(Suit.Spades, Rank.Nine)
        ],
        [
          new Card(Suit.Spades, Rank.Ace),
          new Card(Suit.Spades, Rank.Nine)
        ],
        [
          new Card(Suit.Spades, Rank.Ten),
          new Card(Suit.Hearts, Rank.Nine)
        ],
        [
          new Card(Suit.Spades, Rank.Ace),
          new Card(Suit.Spades, Rank.Nine)
        ]
      ));
    trickTaking.PlayCard(Player.One, new(Suit.Spades, Rank.Ace));
    trickTaking.PlayCard(Player.Two, new(Suit.Spades, Rank.Nine));
    Asserts.ThrowsErrorCode(ErrorCodes.CardNotAllowed,
      () => trickTaking.PlayCard(Player.Three, new(Suit.Hearts, Rank.Nine))
    );
  }
}
