using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks;

public interface ITrickTakingInteractor
{
    void PlayCard(Player player, Card card);
}

public interface ITrickTakingProgress
{
    IReadOnlyList<ITrick> Tricks { get; }
    ICardsByPlayer RemainingCards { get; }
}
