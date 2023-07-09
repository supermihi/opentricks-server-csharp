namespace Doppelkopf.Core.Auctions;

public interface IDeclarableContract
{
  bool IsAllowed(IPlayerCards cards);
  DeclaredContractType Type { get; }
}
