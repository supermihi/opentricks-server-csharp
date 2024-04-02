using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Tricks.Impl;
using Doppelkopf.TestUtils;
using Moq;
using Xunit;

namespace Doppelkopf.Core.Tests.Tricks;

public class PlayCardTests
{
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
}
