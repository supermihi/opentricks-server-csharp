global using CardsByPlayer =
  Doppelkopf.Core.Utils.ByPlayer<System.Collections.Immutable.ImmutableArray<Doppelkopf.Core.Cards.Card>>;
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
    cards.Apply(c => c[..cardsPerPlayer]);
}
