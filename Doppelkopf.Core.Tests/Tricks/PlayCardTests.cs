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
        var provider = Mock.Of<ITrickSuitProvider>();
        var evaluator = Mock.Of<ITrickEvaluator>(MockBehavior.Strict);
        var cards = CardFactory.PlayersCards(cardsPerPlayer: 3, seed: 1245);
        var state = TrickTakingState.Initial(cards);
        var trickTaking = new TrickTaking(evaluator, provider, state);

        var card = cards[Player.One][0];
        trickTaking.PlayCard(Player.One, card);
        Assert.Equal(card, trickTaking.CurrentTrick.Cards[0]);
        Assert.Equal(cards[Player.One][1..], trickTaking.RemainingCards.GetCards(Player.One));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CompletesTrickAndCreatesNewIfNotLast(bool isLastTrick)
    {
        var evaluator = Mock.Of<ITrickEvaluator>(p => p.GetWinner(It.IsAny<ITrick>(), It.IsAny<bool>()) == Player.Four);
        var suitProvider = Mock.Of<ITrickSuitProvider>(p => p.GetTrickSuit(It.IsAny<Card>()) == TrickSuit.Hearts);

        var cards = CardFactory.PlayersCards(cardsPerPlayer: isLastTrick ? 1 : 2, seed: 1234);
        var state = TrickTakingState.Initial(cards);

        var trickTaking = new TrickTaking(evaluator, suitProvider, state);
        for (var i = 0; i < 4; i++)
        {
            var next = trickTaking.CurrentTrick.Cards.Next!.Value;
            trickTaking.PlayCard(next, trickTaking.RemainingCards.GetCards(next).First());
        }
        var completed = trickTaking.Tricks[0];
        Assert.Equal(Player.Four, completed.Winner);
        if (isLastTrick)
        {
            Assert.Single(trickTaking.Tricks);
        }
        else
        {
          Assert.Equal(2, trickTaking.Tricks.Count);
          Assert.Equal(Player.Four, trickTaking.Tricks[^1].Leader);
          Assert.Empty(trickTaking.Tricks[^1].Cards);
        }
    }
}
