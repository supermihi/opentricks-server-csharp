using System.Collections.Immutable;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks.Impl;

public sealed record TrickTakingState(ByPlayer<IImmutableList<Card>> RemainingCards, InTurns<Card>? CurrentTrick,
  ImmutableList<CompletedTrick> CompletedTricks);

public sealed record CompletedTrick(ByPlayer<Card> Cards, Player First, Player Winner);
