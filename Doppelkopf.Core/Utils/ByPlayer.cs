using System.Collections;
using System.Collections.Immutable;

namespace Doppelkopf.Core.Utils;

public static class ByPlayer
{
    public static ByPlayer<T> Init<T>(T initial)
    {
        return Init(_ => initial);
    }

    public static ByPlayer<T> Init<T>(Func<Player, T> getValue)
    {
        return new(Enum.GetValues<Player>().Select(getValue).ToImmutableArray());
    }
}

public sealed record ByPlayer<T> : IByPlayer<T>
{
    private readonly ImmutableArray<T> _values;

    public ByPlayer(T player1, T player2, T player3, T player4)
        : this(ImmutableArray.Create(player1, player2, player3, player4)) { }

    internal ByPlayer(ImmutableArray<T> values)
    {
        _values = values;
    }

    public T this[Player p] => _values[(int)p];

    public ByPlayer<T> Replace(Player player, T value) => new(_values.SetItem((int)player, value));

    public IEnumerable<(Player player, T item)> Items => _values.Select((v, i) => ((Player)i, v));

    public IEnumerator<T> GetEnumerator() => _values.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ByPlayer<TResult> Apply<TResult>(Func<T, TResult> map) =>
        ByPlayer.Init(p => map(this[p]));
}
