using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Games;
using Doppelkopf.Utils;

namespace Doppelkopf.Tests;

public class GameTests
{
  [Fact]
  public void TestInitialState()
  {
    var config = Configuration.Minikopf;
    var cards = config.Cards.ShuffleCards(0);
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
    var afterFirstTrick = game.PlayCard(Player.Player1, new(Suit.Hearts, Rank.Ace))
        .result.PlayCard(Player.Player2, new(Suit.Diamonds, Rank.Ace))
        .result.PlayCard(Player.Player3, new(Suit.Hearts, Rank.Nine))
        .result.PlayCard(Player.Player4, new Card(Suit.Hearts, Rank.King));

    Assert.True(afterFirstTrick.finishedTrick);
    game = afterFirstTrick.result.FinishTrick().result;

    var trick = Assert.Single(game.TrickTaking!.CompleteTricks);
    Assert.Equal(Player.Player2, trick.Winner);

    // trick 2def
    game = game.PlayCard(Player.Player2, new(Suit.Spades, Rank.Ace))
        .result.PlayCard(Player.Player3, new(Suit.Clubs, Rank.Queen))
        .result.PlayCard(Player.Player4, new(Suit.Hearts, Rank.Ace))
        .result.PlayCard(Player.Player1, new(Suit.Clubs, Rank.Queen))
        .result.FinishTrick().result;

    Assert.Equal(2, game.TrickTaking!.CompleteTricks.Count);
    trick = game.TrickTaking!.CompleteTricks.Last();
    Assert.Equal(Player.Player3, trick.Winner);

    // trick 3
    game = game.PlayCard(Player.Player3, new(Suit.Hearts, Rank.Ten))
        .result.PlayCard(Player.Player4, new(Suit.Diamonds, Rank.Ace))
        .result.PlayCard(Player.Player1, new(Suit.Hearts, Rank.Ten))
        .result.PlayCard(Player.Player2, new(Suit.Clubs, Rank.Jack))
        .result.FinishTrick().result;
    Assert.Equal(3, game.TrickTaking!.CompleteTricks.Count);
    trick = game.TrickTaking!.CompleteTricks.Last();
    Assert.Equal(Player.Player1, trick.Winner);

    Assert.True(game.TrickTaking.IsFinished);
    Assert.True(game.IsFinished);
  }
}
