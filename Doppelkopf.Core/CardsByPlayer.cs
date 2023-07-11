using System.Collections.Immutable;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core;

public record CardsByPlayer(ByPlayer<ImmutableArray<Card>> Cards) : ICardsByPlayer
{
    public ImmutableArray<Card> this[Player p] => Cards[p];

    public IReadOnlyCollection<Card> GetCards(Player p) => this[p];

    public CardsByPlayer Remove(Player player, Card card)
    {
        return new(Cards.Replace(player, Cards[player].Remove(card)));
    }
}
