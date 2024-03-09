using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts.Impl;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;
using Xunit;

namespace Doppelkopf.Core.Tests.Contracts;

public class WeddingTests
{
  private static CompleteTrick CreateTrick(Player winner, int index)
  {
    var dummyCards = ByPlayer.Init(new Card(Suit.Diamonds, Rank.Nine));
    return new CompleteTrick(Player.Four, winner, dummyCards, index, 0);
  }

  private static void AssertUndecided(IPartyProvider wedding, Player suitor)
  {
    Assert.Null(wedding.DefiningTrick);
    Assert.Equal(Party.Re, wedding.GetParty(suitor));
    foreach (var player in Enum.GetValues<Player>().Where(p => p != suitor))
    {
      Assert.Null(wedding.GetParty(player));
    }
  }

  [Fact]
  public void AnnouncedWeddingTurnsIntoSoloAfterThreeTricks()
  {
    const Player suitor = Player.Three;
    var wedding = new WeddingContract(TieBreakingMode.FirstWins, suitor, true);
    Assert.Null(wedding.DefiningTrick);
    // 1st trick
    wedding.OnTrickFinished(CreateTrick(suitor, 0));
    Assert.Null(wedding.DefiningTrick);
    // 2nd trick
    wedding.OnTrickFinished(CreateTrick(suitor, 1));
    // 3rd trick
    wedding.OnTrickFinished(CreateTrick(suitor, 2));
    Assert.Equal(2, wedding.DefiningTrick);
  }

  [Theory]
  [InlineData(0, Player.One)]
  [InlineData(1, Player.Three)]
  [InlineData(2, Player.Two)]
  public void AnnouncedWeddingDefinesSpouseWhenTrickWon(int firstForeignTrick, Player winner)
  {
    const Player suitor = Player.Four;
    var wedding = new WeddingContract(TieBreakingMode.FirstWins, suitor, true);
    AssertUndecided(wedding, suitor);
    for (var trick = 0; trick < firstForeignTrick; ++trick)
    {
      wedding.OnTrickFinished(CreateTrick(suitor, trick));
      AssertUndecided(wedding, suitor);
    }

    wedding.OnTrickFinished(CreateTrick(winner, firstForeignTrick));
    Assert.Equal(firstForeignTrick, wedding.DefiningTrick);
    Assert.Equal(Party.Re, wedding.GetParty(winner));
    foreach (var other in Enum.GetValues<Player>().Except(new[] { winner, suitor }))
    {
      Assert.Equal(Party.Contra, wedding.GetParty(other));
    }
  }

  [Fact]
  public void SilentSoloIsDefinedFromStart()
  {
    const Player suitor = Player.One;
    var wedding = new WeddingContract(TieBreakingMode.FirstWins, suitor, false);
    Assert.Equal(0, wedding.DefiningTrick);
    Assert.Equal(Party.Re, wedding.GetParty(Player.One));
    Assert.Equal(Party.Contra, wedding.GetParty(Player.Two));
    Assert.Equal(Party.Contra, wedding.GetParty(Player.Three));
    Assert.Equal(Party.Contra, wedding.GetParty(Player.Four));
  }
}
