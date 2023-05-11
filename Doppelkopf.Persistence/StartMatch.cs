using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Persistence;

public record StartMatch(ByPlayer<IImmutableList<Card>> Cards) : ITableActionPayload;
