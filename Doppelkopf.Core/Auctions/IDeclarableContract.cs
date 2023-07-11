using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Auctions;

public interface IDeclarableContract
{
    bool IsAllowed(IEnumerable<Card> playerCards);
    DeclaredContractType Type { get; }
}
