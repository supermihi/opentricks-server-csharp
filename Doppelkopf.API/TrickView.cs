using Doppelkopf.Core;
using Doppelkopf.Core.Cards;

namespace Doppelkopf.API;

public record TrickView(Player Leader, IReadOnlyList<Card> Cards, int Index, Player? Winner);
