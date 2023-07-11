using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Tricks.Impl;
using Doppelkopf.Core.Utils;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace Doppelkopf.Core.Tests.Tricks;

public class TrickUtilsTest
{
    [Fact]
    public void GetWinnerThrowsInvalidOperationOnNotFullTrick()
    {
        var provider = new Mock<ICardTraitsProvider>();
        var trick = new InTurns<Card>(Player.Three, new Card(Suit.Diamonds, Rank.Jack));
        Assert.Throws<InvalidOperationException>(
            () => TrickUtils.GetTrickWinner(provider.Object, trick, false)
        );
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PassesLastTrickProperty(bool isLastTrick)
    {
      var card = new Card(Suit.Clubs, Rank.Ace); // card doesn't matter here
      var provider = new Mock<ICardTraitsProvider>();
      provider.Setup(p => p.TakesTrickFrom(card, card, !isLastTrick))
          .Throws(new FailException("unexpected isLastTrick"));
      var trick = new InTurns<Card>(
        Player.Four, card, card, card, card
        );
      TrickUtils.GetTrickWinner(provider.Object, trick, isLastTrick);
    }
}
