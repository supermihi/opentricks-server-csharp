using System.Collections;

namespace Doppelkopf.Utils;

public sealed record InTurns<T> : IReadOnlyCollection<T>
{
  public Player Start { get; }
  public int Count { get; }
  private readonly ByPlayer<T> _values;
  public bool IsFull => Count == Constants.NumberOfPlayers;

  public InTurns(Player start) : this(start, 0, ByPlayer.Init((T)default!))
  { }

  private InTurns(Player start, int count, ByPlayer<T> values)
  {
    Start = start;
    Count = count;
    _values = values;
  }

  public Player? Next => IsFull ? null : Start.Skip(Count);

  public InTurns<T> Add(T element)
  {
    if (Next is { } next)
    {
      return new(Start, Count + 1, _values.Replace(next, element));
    }
    throw new ArgumentException("cannot add: full");
  }

  public bool Contains(Player p) => p.DistanceFrom(Start) < Count;

  public T this[Player p] => Contains(p) ? _values[p] : throw new IndexOutOfRangeException();
  public T this[int index] => this[Start.Skip(index)];

  public bool TryGet(Player p, out T value)
  {
    value = _values[p];
    return Contains(p);
  }
  public IEnumerable<Player> Players => Start.Cycle().Take(Count);
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public IEnumerator<T> GetEnumerator() => Players.Select(p => _values[p]).GetEnumerator();
}
