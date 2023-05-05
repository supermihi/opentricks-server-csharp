using System.Collections;

namespace Doppelkopf;

public static class ByPlayer
{
  public static ByPlayer<T> Init<T>(T initial)
  {
    return new(initial, initial, initial, initial);
  }
}

public sealed record ByPlayer<T>(T Player1, T Player2, T Player3, T Player4) : IEnumerable<T>
{
  public T Get(Player player) =>
    player switch
    {
      Player.Player1 => Player1,
      Player.Player2 => Player2,
      Player.Player3 => Player3,
      Player.Player4 => Player4,
      _ => throw new ArgumentOutOfRangeException(nameof(player))
    };

  public T this[Player p] => Get(p);

  public ByPlayer<T> Replace(Player player, T value) =>
    new(
      player == Player.Player1 ? value : Player1,
      player == Player.Player2 ? value : Player2,
      player == Player.Player3 ? value : Player3,
      player == Player.Player4 ? value : Player4
    );

  public IEnumerator<T> GetEnumerator()
  {
    yield return Player1;
    yield return Player2;
    yield return Player3;
    yield return Player4;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
