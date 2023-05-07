using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Persistence;

public record StartMatchAction(ByPlayer<IImmutableList<Card>> Cards, int Version) : ITableAction;