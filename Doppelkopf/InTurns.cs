using System.Collections;
using System.Collections.Immutable;

namespace Doppelkopf;

public record InTurns<T>(Player First, IImmutableList<T> Elements) : IEnumerable<T>
{
    public InTurns(Player First)
      : this(First, ImmutableList<T>.Empty) { }

    public Player? Next => IsFull ? null : First.Skip(Elements.Count);

    public InTurns<T> Add(T element)
    {
        if (IsFull)
        {
            throw new ArgumentException("cannot add: full");
        }
        return this with { Elements = Elements.Add(element) };
    }

    public T this[Player p] => Elements[p.DistanceFrom(First)];

    public bool IsFull => Elements.Count == Constants.NumberOfPlayers;

    public IEnumerator<T> GetEnumerator()
    {
        return Elements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class InTurnsExtensions
{
    public static IEnumerable<(Player player, T value)> Items<T>(this InTurns<T> self)
    {
        return self.First.Cycle().Zip(self.Elements);
    }

    public static IEnumerable<Player> PlayersWhere<T>(this InTurns<T> self, Predicate<T> predicate)
    {
        return self.Items().Where(tuple => predicate(tuple.value)).Select(tuple => tuple.player);
    }
}
