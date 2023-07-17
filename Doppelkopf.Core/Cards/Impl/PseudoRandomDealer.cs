using System.Collections.Immutable;

namespace Doppelkopf.Core.Cards.Impl;

public class PseudoRandomDealer : IDealer
{
  private readonly int? _baseSeed;
  private readonly ImmutableArray<Card> _deck;

  public PseudoRandomDealer(IEnumerable<Card> deck, int? baseSeed)
  {
    _baseSeed = baseSeed;
    _deck = deck.ToImmutableArray();
  }

  public CardsByPlayer ShuffleCards(int index)
  {
    var random = _baseSeed is { } seed ? new Random(seed + index) : Random.Shared;
    return random.Shuffle(_deck);
  }
}
