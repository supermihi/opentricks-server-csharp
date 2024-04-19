using Doppelkopf.Core;

namespace Doppelkopf.API;

public record AuctionView(Player Leader, IReadOnlyList<bool> Holds);
