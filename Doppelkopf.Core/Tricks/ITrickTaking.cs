using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks;

public interface ITrickTaking
{
    void PlayCard(Player player, Card card);
    InTurns<Card>? CurrentTrick { get; }
    ICardsByPlayer RemainingCards { get; }
}
