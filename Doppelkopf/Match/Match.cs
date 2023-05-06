using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.GameFinding;
using Doppelkopf.Tricks;

namespace Doppelkopf.Match;

public sealed record Match(ByPlayer<IImmutableList<Card>> Cards, IRules Rules, Auction Auction,
    TrickTaking? TrickTaking);