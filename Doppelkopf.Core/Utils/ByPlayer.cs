using System.Collections;
using System.Collections.Immutable;

namespace Doppelkopf.Core.Utils;

public static class ByPlayer
{
  public static ByPlayer<T> Init<T>(T initial) => Init(_ => initial);

  public static ByPlayer<T> Init<T>(Func<Player, T> getValue) =>
    new(Enum.GetValues<Player>().Select(getValue).ToImmutableArray());
}

public record ByPlayer<T> : IReadOnlyDictionary<Player, T>
{
  private readonly ImmutableArray<T> _values;

  public ByPlayer(T player1, T player2, T player3, T player4)
    : this(ImmutableArray.Create(player1, player2, player3, player4))
  {
  }

  internal ByPlayer(ImmutableArray<T> values) => _values = values;

  public bool ContainsKey(Player key) => true;

  public bool TryGetValue(Player key, out T value)
  {
    value = this[key];
    return true;
  }

  public T this[Player p] => _values[(int)p];
  public IEnumerable<Player> Keys => Enum.GetValues<Player>();
  public IEnumerable<T> Values => _values;

  public ByPlayer<T> Replace(Player player, T value) => new(_values.SetItem((int)player, value));

  public IEnumerable<(Player player, T value)> Items => _values.Select((v, i) => ((Player)i, v));

  public IEnumerator<KeyValuePair<Player, T>> GetEnumerator() =>
    Items.Select(i => KeyValuePair.Create(i.player, i.value)).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public ByPlayer<TResult> Apply<TResult>(Func<T, TResult> map) => ByPlayer.Init(p => map(this[p]));
  public int Count => Rules.NumPlayers;
}
