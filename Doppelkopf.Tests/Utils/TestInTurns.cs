using Doppelkopf.Utils;

namespace Doppelkopf.Tests.Utils;

public class TestInTurns
{
  [Fact]
  public void InitialInTurnsIsEmpty()
  {
    var turns = new InTurns<int>(Player.Player3);
    Assert.Empty(turns);
    Assert.All(Enum.GetValues<Player>(), p => Assert.False(turns.Contains(p)));
  }

  [Fact]
  public void CanAccessOnlySetItems()
  {
    var turns = new InTurns<int>(Player.Player2)
        .Add(17 /* player 2 */)
        .Add(42 /* player 3 */);

    Assert.Equal(2, turns.Count);
    Assert.Equal(17, turns[Player.Player2]);
    Assert.Equal(17, turns[0]);
    Assert.Equal(42, turns[Player.Player3]);
    Assert.Equal(42, turns[1]);

    Assert.Throws<IndexOutOfRangeException>(() => turns[2]);
    Assert.Throws<IndexOutOfRangeException>(() => turns[Player.Player4]);
    Assert.Throws<IndexOutOfRangeException>(() => turns[3]);
    Assert.Throws<IndexOutOfRangeException>(() => turns[Player.Player1]);
  }

}
