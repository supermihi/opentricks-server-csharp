using Doppelkopf.Games;

namespace Doppelkopf.Server.Interface;

public sealed record JsonAuction(JsonByPlayer<bool?> Reservations, JsonByPlayer<string?> Declarations)
{
  public static JsonAuction FromAuction(Auction auction, Player? maskFor)
  {
    var reservations = JsonByPlayer.FromInTurns(auction.Reservations, (_, reserved) => (bool?)reserved, null);
    var declarations = JsonByPlayer.FromByPlayer(
      auction.Declarations,
      (player, contract) => player == maskFor ? contract?.Id : contract is null ? null : "?");
    return new(reservations, declarations);
  }
}