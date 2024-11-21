using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Auctions;

public interface IAuction
{
  /// <summary>
  /// Whose turn it is. <c>null</c> if and only if the auction is finished.
  /// </summary>
  Player? Turn { get; }

  void Declare(Player player, Declaration declaration);
  InTurns<Declaration> Declarations { get; }
  AuctionResult? Evaluate();
}
