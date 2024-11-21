namespace Doppelkopf.API.Views;

public sealed record AuctionView(Player Leader, IReadOnlyList<bool> Holds);
