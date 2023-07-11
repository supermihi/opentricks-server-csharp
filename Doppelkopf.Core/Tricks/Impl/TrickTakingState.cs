using System.Collections.Immutable;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks.Impl;

internal sealed record TrickTakingState(CardsByPlayer RemainingCards,
  InTurns<Card>? CurrentTrick,
  ImmutableList<CompletedTrick> CompletedTricks)
{
  public static TrickTakingState Initial(CardsByPlayer initialCards) =>
      new(
        initialCards,
        new InTurns<Card>(Player.One),
        ImmutableList<CompletedTrick>.Empty);
}
