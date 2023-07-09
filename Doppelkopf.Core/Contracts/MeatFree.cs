using Doppelkopf.Core.Auctions;

namespace Doppelkopf.Core.Contracts;

public class MeatFree : IDeclarableContract
{
  public bool IsAllowed(IPlayerCards cards) => true;
  public DeclaredContractType Type => DeclaredContractType.Solo;
}
