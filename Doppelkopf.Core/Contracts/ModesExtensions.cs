using Doppelkopf.Core.Auctions;

namespace Doppelkopf.Core.Contracts;

public static class ModesExtensions
{
  public static IContract CreateContract(this Modes modes, AuctionResult result, CardsByPlayer dealtCards)
  {
    if (result.Hold is { } hold)
    {
      return hold.CreateContract(result.Declarer!.Value);
    }

    return modes.NormalGame.CreateNormalGame(dealtCards);
  }
}
