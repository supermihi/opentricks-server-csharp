using Doppelkopf.Core.Auctions;

namespace Doppelkopf.Core.Contracts;

public static class ModesExtensions
{
  public static IContract CreateContract(this Modes modes, AuctionResult result, ICardsByPlayer dealtCards)
  {
    if (result.Hold is { } hold)
    {
      return hold.CreateContract(result.Declarer!.Value, dealtCards);
    }

    return modes.NormalGame.CreateNormalGame(dealtCards);
  }
}
