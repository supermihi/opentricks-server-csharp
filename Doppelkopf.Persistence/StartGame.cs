using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Persistence;

public record StartGame(ByPlayer<IImmutableList<Card>> Cards) : ITableActionPayload;
