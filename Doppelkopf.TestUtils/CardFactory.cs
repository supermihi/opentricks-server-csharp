using Doppelkopf.Core;
using Doppelkopf.Core.Cards;

namespace Doppelkopf.TestUtils;

public static class CardFactory
{
    public static CardsByPlayer PlayersCards(int cardsPerPlayer, int seed)
    {
        var random = new Random(seed);
        var deck = random.Shuffle(Decks.WithNines);
        var reduced = deck.Cards.Apply(cards => cards[..cardsPerPlayer]);
        return new(reduced);
    }
}
