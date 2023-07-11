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
        var cards = CardFactory.PlayersCards(cardsPerPlayer: 3, seed: 1245);
        var state = TrickTakingState.Initial(cards);
        var trickTaking = new TrickTaking(provider, state);

        var card = cards[Player.One][0];
        trickTaking.PlayCard(Player.One, card);
        Assert.Equal(card, trickTaking.CurrentTrick![0]);
        Assert.Equal(cards[Player.One][1..], trickTaking.RemainingCards.GetCards(Player.One));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CompletesTrickAndCreatesNewIfNotLast(bool isLastTrick)
    {
        var provider = Mock.Of<ICardTraitsProvider>(
            // last card always wins
            p =>
                p.TakesTrickFrom(It.IsAny<Card>(), It.IsAny<Card>(), It.IsAny<bool>()) == true
                && p.GetTraits(It.IsAny<Card>()) == new CardTraits(TrickSuit.Hearts, 1)
        );
        var cards = CardFactory.PlayersCards(cardsPerPlayer: isLastTrick ? 1 : 2, seed: 1234);
        var state = TrickTakingState.Initial(cards);

        var trickTaking = new TrickTaking(provider, state);
        for (var i = 0; i < 4; i++)
        {
            var next = trickTaking.CurrentTrick!.Next!.Value;
            trickTaking.PlayCard(next, trickTaking.RemainingCards.GetCards(next).First());
        }
        var completed = Assert.Single(trickTaking.CompletedTricks);
        Assert.Equal(Player.Four, completed.Winner);
        if (isLastTrick)
        {
            Assert.Null(trickTaking.CurrentTrick);
        }
        else
        {
            Assert.Empty(trickTaking.CurrentTrick!);
            Assert.Equal(Player.Four, trickTaking.CurrentTrick!.Start);
        }
    }
}
