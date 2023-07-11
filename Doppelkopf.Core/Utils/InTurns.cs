using System.Collections;
using System.Collections.Immutable;

namespace Doppelkopf.Core.Utils;

public sealed record InTurns<T> : IEnumerable<T> {
  public Player Start { get; }
  public int Count => _values.Length;
  private readonly ImmutableArray<T> _values;
  public bool IsFull => Count == Rules.NumPlayers;

  public InTurns(Player start) : this(start, ImmutableArray.Create<T>())
  {
  }

  public InTurns(Player start, params T[] values) : this(start, values.ToImmutableArray())
  { }

  public InTurns(Player start, IEnumerable<T> values) : this(start, values.ToImmutableArray())
  { }

  private InTurns(Player start, ImmutableArray<T> values)
  {
    if (values.Length > Rules.NumPlayers)
    {
      throw new ArgumentException("too many values", nameof(values));
    }
    Start = start;
    _values = values;
  }

  public Player? Next => IsFull ? null : Start.Skip(Count);

  public InTurns<T> Add(T element)
  {
    if (IsFull)
    {
      throw new ArgumentException("cannot add: full");
    }
    return new(Start, _values.Add(element));
  }

  public bool Contains(Player p) => p.DistanceFrom(Start) < Count;

  public T this[Player p] => this[p.DistanceFrom(Start)];
  public T this[int index] => index < Count ? _values[index] : throw new KeyNotFoundException();

  public bool TryGet(Player p, out T value)
  {
    if (Contains(p))
    {
      value = this[p];
      return true;
    }
    value = default!;
    return false;
  }
  public IEnumerable<(Player player, T item)> Items => _values.Select((v, i) => ((Player)i, v));
  public IEnumerable<Player> Players => Start.Cycle().Take(Count);
  public IEnumerator<T> GetEnumerator() => _values.AsEnumerable().GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}