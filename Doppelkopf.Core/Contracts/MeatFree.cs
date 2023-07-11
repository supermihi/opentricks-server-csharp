using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Contracts;

public class MeatFree : IDeclarableContract
{
    public bool IsAllowed(IEnumerable<Card> cards) => true;

    public DeclaredContractType Type => DeclaredContractType.Solo;
}
