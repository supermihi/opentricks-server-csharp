using Doppelkopf.Core;

namespace Doppelkopf.API;

public sealed record AuctionView(Player Leader, IReadOnlyList<bool> Holds);
