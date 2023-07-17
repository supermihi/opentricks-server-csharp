using Doppelkopf.Core.Utils;
using Xunit;

namespace Doppelkopf.Core.Tests;

public class PlayerExtensionsTests
{
  [Theory]
  [InlineData(Player.One)]
  [InlineData(Player.Two)]
  public void SkipZeroReturnsPlayer(Player player)
  {
    var skipped = player.Skip(0);
    Assert.Equal(player, skipped);
  }

  [Fact]
  public void TestCycle()
  {
    var cycle = Player.Three.Cycle().Take(6);
    var expected = new[] { Player.Three, Player.Four, Player.One, Player.Two, Player.Three, Player.Four };
    Assert.Equal(expected, cycle);
  }
}
