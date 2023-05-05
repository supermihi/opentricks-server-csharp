using System.Collections.Immutable;

namespace Doppelkopf;

public sealed record InTurns<T>(Player First, ImmutableList<T> Elements)
{
  public InTurns(Player First)
    : this(First, ImmutableList<T>.Empty) { }
}
