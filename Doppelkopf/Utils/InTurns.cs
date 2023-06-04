using System.Collections;
using System.Collections.Immutable;

namespace Doppelkopf.Utils;

public sealed record InTurns<T> : IReadOnlyCollection<T>
{
  public Player Start { get; }
  public int Count => _values.Count();
  private readonly ImmutableArray<T> _values;
  public bool IsFull => Count == Constants.NumberOfPlayers;

  public InTurns(Player start) : this(start, ImmutableArray.Create<T>())
  { }

  private InTurns(Player start, ImmutableArray<T> values)
  {
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
  public T this[int index] => index < Count ? _values[index] : throw new IndexOutOfRangeException();

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
  public IEnumerable<Player> Players => Start.Cycle().Take(Count);
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  public IEnumerator<T> GetEnumerator() => _values.AsEnumerable().GetEnumerator();
}
