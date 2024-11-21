using Doppelkopf.Core.Cards;
using Xunit;

namespace Doppelkopf.Core.Tests.Cards;

public class TestDecks
{
  [Fact]
  public void DeckWithoutNines()
  {
    Assert.Equal(40, Decks.WithoutNines.Count);

    var sameCards = Decks.WithoutNines.GroupBy(c => c);
    Assert.All(sameCards, c => Assert.Equal(2, c.Count()));

    foreach (var suit in Enum.GetValues<Suit>())
    {
      foreach (var rank in Enum.GetValues<Rank>().Except([Rank.Nine]))
      {
        Assert.Contains(new Card(suit, rank), Decks.WithoutNines);
      }
    }
  }

  [Fact]
  public void DeckWithNines()
  {
    Assert.Equal(48, Decks.WithNines.Count);

    var sameCards = Decks.WithNines.GroupBy(c => c);
    Assert.All(sameCards, c => Assert.Equal(2, c.Count()));

    foreach (var suit in Enum.GetValues<Suit>())
    {
      foreach (var rank in Enum.GetValues<Rank>())
      {
        Assert.Contains(new Card(suit, rank), Decks.WithNines);
      }
    }
  }
}
