using Doppelkopf.Core.Cards;
using Xunit;

namespace Doppelkopf.Core.Tests.Cards;

public class TestShuffle
{
  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void ShuffleReturnsAllCards(bool withNines)
  {
    var deck = withNines ? Decks.WithNines : Decks.WithoutNines;
    var shuffledCards = Random.Shared.Shuffle(deck);
    var allCards = Enum.GetValues<Player>()
      .SelectMany(p => shuffledCards[p])
      .ToHashSet();

    Assert.Equal(deck.ToHashSet(), allCards);
  }
}
