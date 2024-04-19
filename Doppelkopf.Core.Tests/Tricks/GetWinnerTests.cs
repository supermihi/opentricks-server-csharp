using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Tricks.Impl;
using Doppelkopf.Core.Utils;
using Xunit;

namespace Doppelkopf.Core.Tests.Tricks;

public class GetWinnerTests
{
  [Fact]
  public void SideSuitAceWinsInNormalGame()
  {
    var trick = new Trick(
      new InTurns<Card>(
        Player.One,
        new(Suit.Spades, Rank.King),
        new(Suit.Spades, Rank.Ace),
        new(Suit.Hearts, Rank.Nine),
        new(Suit.Spades, Rank.Ace)),
      0,
      12);
    var winner = trick.GetWinner(CardTraitsProvider.NormalGame(TieBreakingMode.FirstWins));
    Assert.Equal(Player.Two, winner);
  }

  [Theory]
  [InlineData(TieBreakingMode.FirstWins, false)]
  [InlineData(TieBreakingMode.FirstWins, true)]
  [InlineData(TieBreakingMode.SecondWins, false)]
  [InlineData(TieBreakingMode.SecondWins, true)]
  [InlineData(TieBreakingMode.SecondWinsInLastTrick, false)]
  [InlineData(TieBreakingMode.SecondWinsInLastTrick, true)]
  public void TieBreakingModeIsRespected(TieBreakingMode heartsTen, bool isLastTrick)
  {
    var trick = new Trick(
      new InTurns<Card>(
        Player.One,
        new(Suit.Spades, Rank.King),
        new(Suit.Hearts, Rank.Ten),
        new(Suit.Hearts, Rank.Ten),
        new(Suit.Spades, Rank.Ace)),
      10,
      isLastTrick ? 0 : 2);
    var winner = trick.GetWinner(CardTraitsProvider.NormalGame(heartsTen));
    var expectedWinner =
      heartsTen == TieBreakingMode.SecondWins || (heartsTen == TieBreakingMode.SecondWinsInLastTrick && isLastTrick)
        ? Player.Three
        : Player.Two;
    Assert.Equal(expectedWinner, winner);
  }
}
