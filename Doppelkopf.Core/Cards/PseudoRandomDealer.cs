using System.Collections.Immutable;

namespace Doppelkopf.Core.Cards;

internal class PseudoRandomDealer(IEnumerable<Card> deck, int? baseSeed) : IDealer
{
  private readonly ImmutableArray<Card> _deck = deck.ToImmutableArray();

  public CardsByPlayer ShuffleCards(int index)
  {
    var random = baseSeed is { } seed ? new Random(seed + index) : Random.Shared;
    return random.Shuffle(_deck);
  }
}
