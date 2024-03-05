using Doppelkopf.Core.Cards;

namespace Doppelkopf.Core.Tricks;

public interface ITrickTakingInteractor
{
  CompleteTrick? PlayCard(Player player, Card card);
}