using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks;

public interface ITrickTakingInteractor
{
  CompleteTrick? PlayCard(Player player, Card card);
}

public interface ITrickTakingProgress
{
  IReadOnlyList<CompleteTrick> CompleteTricks { get; }
  Trick? CurrentTrick { get; }
  ICardsByPlayer RemainingCards { get; }
}
