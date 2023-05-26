using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Conf;
using Doppelkopf.Games;

namespace Doppelkopf.Tests;

public class GameTests
{
  [Fact]
  public void TestInitialState()
  {
    var config = Configuration.Minikopf;
    var cards = config.Deck.Shuffle(Random.Shared);
    var context = new GameContext(ByPlayer.Init(false), config.Contracts, config.Tricks);
    var game = Game.Init(context, cards);

    Assert.Equal(cards, game.Cards);
    Assert.Null(game.TrickTaking);
    Assert.Empty(game.Auction.Reservations);
    Assert.All(game.Auction.Declarations, Assert.Null);
  }

  [Fact]
  public void TestCompleteGame()
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
    var minikopf = Configuration.Minikopf;
    var context = new GameContext(
      new ByPlayer<bool>(true, false, false, false),
      Configuration.Minikopf.Contracts,
      minikopf.Tricks
    );
    var game = Game.Init(context, cards);

    game = game.Reserve(Player.Player1, false)
        .Reserve(Player.Player2, false)
        .Reserve(Player.Player3, false)
        .Reserve(Player.Player4, false);

    Assert.NotNull(game.TrickTaking);
    Assert.Equal(Configuration.Minikopf.Contracts.NormalGame.Id, game.TrickTaking.Contract.Id);

    // trick 1
    game = game.PlayCard(Player.Player1, new(Suit.Hearts, Rank.Ace))
        .PlayCard(Player.Player2, new(Suit.Diamonds, Rank.Ace))
        .PlayCard(Player.Player3, new(Suit.Hearts, Rank.Nine))
        .PlayCard(Player.Player4, new Card(Suit.Hearts, Rank.King));

    var trick = Assert.Single(game.TrickTaking!.CompletedTricks);
    Assert.Equal(Player.Player2, trick.Winner);

    // trick 2def
    game = game.PlayCard(Player.Player2, new(Suit.Spades, Rank.Ace))
        .PlayCard(Player.Player3, new(Suit.Clubs, Rank.Queen))
        .PlayCard(Player.Player4, new(Suit.Hearts, Rank.Ace))
        .PlayCard(Player.Player1, new(Suit.Clubs, Rank.Queen));

    Assert.Equal(2, game.TrickTaking!.CompletedTricks.Count);
    trick = game.TrickTaking!.CompletedTricks.Last();
    Assert.Equal(Player.Player3, trick.Winner);

    // trick 3
    game = game.PlayCard(Player.Player3, new(Suit.Hearts, Rank.Ten))
        .PlayCard(Player.Player4, new(Suit.Diamonds, Rank.Ace))
        .PlayCard(Player.Player1, new(Suit.Hearts, Rank.Ten))
        .PlayCard(Player.Player2, new(Suit.Clubs, Rank.Jack));
    Assert.Equal(3, game.TrickTaking!.CompletedTricks.Count);
    trick = game.TrickTaking!.CompletedTricks.Last();
    Assert.Equal(Player.Player1, trick.Winner);

    Assert.True(game.TrickTaking.IsFinished);
  }
}
