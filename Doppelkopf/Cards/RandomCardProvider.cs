using System.Collections.Immutable;

namespace Doppelkopf.Cards;

public class RandomCardProvider : ICardProvider
{
  private readonly Random _random;
  public IImmutableList<Card> Deck { get; }

  public RandomCardProvider(IImmutableList<Card> deck, Random? random = null)
  {
    Deck = deck;
    _random = random ?? Random.Shared;
  }

  public ByPlayer<IImmutableList<Card>> ShuffleCards(int gameIndex) => Deck.Shuffle(_random);
  public Card GetById(string cardId) => Deck.First(c => c.Id == cardId);
}
