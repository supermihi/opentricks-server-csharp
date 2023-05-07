using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;

namespace Doppelkopf.Tests;

public class MatchTests
{
  [Fact]
  public void TestInitial()
  {
    var cards = Rules.Minikopf.Deck.Shuffle(Random.Shared);
    var match = Match.Init(cards);

    Assert.Equal(cards, match.Cards);
    Assert.Null(match.TrickTaking);
    Assert.Empty(match.Auction.Reservations);
    Assert.All(match.Auction.Declarations, Assert.Null);
  }

  [Fact]
  public void TestCompleteMatchNormalGame()
  {
    var cards = new ByPlayer<IImmutableList<Card>>(
      ImmutableArray.Create(
        new Card(Suit.Hearts, Rank.Ten),
        new Card(Suit.Clubs, Rank.Queen),
        new Card(Suit.Hearts, Rank.Ace)
      ),
      ImmutableArray.Create(
        new Card(Suit.Clubs, Rank.Jack),
        new Card(Suit.Diamonds, Rank.Ace),
        new Card(Suit.Spades, Rank.Ace)
      ),
      ImmutableArray.Create(
        new Card(Suit.Hearts, Rank.Ten),
        new Card(Suit.Clubs, Rank.Queen),
        new Card(Suit.Hearts, Rank.Nine)
      ),
      ImmutableArray.Create(
        new Card(Suit.Diamonds, Rank.Ace),
        new Card(Suit.Hearts, Rank.Ace),
        new Card(Suit.Hearts, Rank.King)
      )
    );
    var match = Match.Init(cards);
    var context = new MatchContext(
      new ByPlayer<bool>(true, false, false, false),
      Rules.Minikopf.Modes
    );

    match = match
      .Reserve(Player.Player1, false, context)
      .Reserve(Player.Player2, false, context)
      .Reserve(Player.Player3, false, context)
      .Reserve(Player.Player4, false, context);

    Assert.NotNull(match.TrickTaking);
    Assert.Equal(Rules.Minikopf.Modes.NormalGame, match.TrickTaking.Contract.Mode);

    // trick 1
    match = match
      .PlayCard(Player.Player1, new(Suit.Hearts, Rank.Ace))
      .PlayCard(Player.Player2, new(Suit.Diamonds, Rank.Ace))
      .PlayCard(Player.Player3, new(Suit.Hearts, Rank.Nine))
      .PlayCard(Player.Player4, new Card(Suit.Hearts, Rank.King));

    var trick = Assert.Single(match.TrickTaking!.CompletedTricks);
    Assert.Equal(Player.Player2, trick.Winner);

    // trick 2def
    match = match
      .PlayCard(Player.Player2, new(Suit.Spades, Rank.Ace))
      .PlayCard(Player.Player3, new(Suit.Clubs, Rank.Queen))
      .PlayCard(Player.Player4, new(Suit.Hearts, Rank.Ace))
      .PlayCard(Player.Player1, new(Suit.Clubs, Rank.Queen));

    Assert.Equal(2, match.TrickTaking!.CompletedTricks.Count);
    trick = match.TrickTaking!.CompletedTricks.Last();
    Assert.Equal(Player.Player3, trick.Winner);

    // trick 3
    match = match
      .PlayCard(Player.Player3, new(Suit.Hearts, Rank.Ten))
      .PlayCard(Player.Player4, new(Suit.Diamonds, Rank.Ace))
      .PlayCard(Player.Player1, new(Suit.Hearts, Rank.Ten))
      .PlayCard(Player.Player2, new(Suit.Clubs, Rank.Jack));
    Assert.Equal(3, match.TrickTaking!.CompletedTricks.Count);
    trick = match.TrickTaking!.CompletedTricks.Last();
    Assert.Equal(Player.Player1, trick.Winner);

    Assert.True(match.TrickTaking.IsFinished);
  }
}
