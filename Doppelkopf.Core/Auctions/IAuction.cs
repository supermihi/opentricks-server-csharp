namespace Doppelkopf.Core.Auctions;

public interface IAuction
{
  /// <summary>
  /// Whose turn it is. <c>null</c> if and only if the auction is finished.
  /// </summary>
  Player? Turn { get; }

  void DeclareReservation(Player player, IDeclarableContract contract);
  void DeclareOk(Player player);
  AuctionResult? Evaluate();
}
