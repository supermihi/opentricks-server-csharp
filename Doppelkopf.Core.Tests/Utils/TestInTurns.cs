using Doppelkopf.Core.Utils;
using Xunit;

namespace Doppelkopf.Core.Tests.Utils;

public class TestInTurns
{
  [Fact]
  public void InitialInTurnsIsEmpty()
  {
    var turns = new InTurns<int>(Player.Three);
    Assert.Empty(turns);
    Assert.All(Enum.GetValues<Player>(), p => Assert.False(turns.Contains(p)));
  }

  [Fact]
  public void CanAccessOnlySetItems()
  {
    var turns = new InTurns<int>(Player.Two)
      .Add(17 /* player 2 */)
      .Add(42 /* player 3 */);

    Assert.Equal(2, turns.Count);
    Assert.Equal(17, turns[Player.Two]);
    Assert.Equal(17, turns[0]);
    Assert.Equal(42, turns[Player.Three]);
    Assert.Equal(42, turns[1]);

    Assert.Throws<KeyNotFoundException>(() => turns[2]);
    Assert.Throws<KeyNotFoundException>(() => turns[Player.Four]);
    Assert.Throws<KeyNotFoundException>(() => turns[3]);
    Assert.Throws<KeyNotFoundException>(() => turns[Player.One]);
  }
}
