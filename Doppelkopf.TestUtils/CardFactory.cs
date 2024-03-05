using Doppelkopf.Core;
using Doppelkopf.Core.Cards;

namespace Doppelkopf.TestUtils;

public static class CardFactory
{
  public static CardsByPlayer PlayersCards(bool withNines, int seed)
  {
    var random = new Random(seed);
    return random.Shuffle(Decks.WithNines);
  }

  public static CardsByPlayer Reduce(this CardsByPlayer cards, int cardsPerPlayer) =>
    new(cards.Cards.Apply(c => c[..cardsPerPlayer]));
}
