using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts.Impl;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;
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
    var normalGame = new NormalGameContract(mode, ByPlayer.Init(false));
    var heartsTenTraits = normalGame.GetTraits(Card.HeartsTen);
    Assert.Equal(mode, heartsTenTraits.TieBreaking);
  }

  [Fact]
  public void NormalGameDerivesPartiesFromInput()
  {
    var game = new NormalGameContract(TieBreakingMode.FirstWins, new ByPlayer<bool>(true, false, false, true));
    Assert.Equal(0, game.DefiningTrick);
    Assert.Equal(Party.Re, game.GetParty(Player.One));
    Assert.Equal(Party.Contra, game.GetParty(Player.Two));
    Assert.Equal(Party.Contra, game.GetParty(Player.Three));
    Assert.Equal(Party.Re, game.GetParty(Player.Four));
  }
}
