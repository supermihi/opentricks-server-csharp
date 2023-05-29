using System.Collections;

namespace Doppelkopf;

public static class ByPlayer
{
  public static ByPlayer<T> Init<T>(T initial)
  {
    return Init(_ => initial);
  }

  public static ByPlayer<T> Init<T>(Func<Player, T> getValue)
  {
    return new(
      getValue(Player.Player1),
      getValue(Player.Player2),
      getValue(Player.Player3),
      getValue(Player.Player4)
    );
  }
}

public sealed record ByPlayer<T>(T Player1, T Player2, T Player3, T Player4) : IEnumerable<T>
{
  public T this[Player p] => p switch
  {
    Player.Player1 => Player1,
    Player.Player2 => Player2,
    Player.Player3 => Player3,
    Player.Player4 => Player4,
    _ => throw new ArgumentOutOfRangeException(nameof(p))
  };

  public ByPlayer<T> Replace(Player player, T value) =>
      new(
        player == Player.Player1 ? value : Player1,
        player == Player.Player2 ? value : Player2,
        player == Player.Player3 ? value : Player3,
        player == Player.Player4 ? value : Player4
      );

  public IEnumerable<(Player player, T item)> Items => Enum.GetValues<Player>().Zip(this);

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
