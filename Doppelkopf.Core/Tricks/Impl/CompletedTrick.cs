using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks.Impl;

internal sealed record CompletedTrick(ByPlayer<Card> Cards, Player First, Player Winner);
