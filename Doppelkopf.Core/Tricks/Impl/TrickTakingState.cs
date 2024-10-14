using System.Collections.Immutable;

namespace Doppelkopf.Core.Tricks.Impl;

internal sealed record TrickTakingState(CardsByPlayer RemainingCards, ImmutableArray<CompleteTrick> CompleteTricks,
  Trick? CurrentTrick)
{
  public static TrickTakingState Initial(CardsByPlayer initialCards) =>
      new(
        initialCards,
        [],
        new Trick(Player.One, 0, initialCards[Player.One].Length - 1)
      );
}
