using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts.Impl;
using Doppelkopf.Core.Tricks;
using Doppelkopf.TestUtils;
using Xunit;

namespace Doppelkopf.Core.Tests.Contracts;

public class NormalGameTests
{
  [Theory]
  [InlineData(TieBreakingMode.FirstWins)]
  [InlineData(TieBreakingMode.SecondWins)]
  [InlineData(TieBreakingMode.SecondWinsInLastTrick)]
  public void NormalGameRespectsHeartsTenTieBrakingMode(TieBreakingMode mode)
  {
    var normalGame = new NormalGame(mode);
    var heartsTenTraits = normalGame.CardTraits.GetTraits(Card.HeartsTen);
    Assert.Equal(mode, heartsTenTraits.TieBreaking);
  }
}
