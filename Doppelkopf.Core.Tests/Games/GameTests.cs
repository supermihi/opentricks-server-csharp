using System.Collections.Immutable;
using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Games.Impl;
using Doppelkopf.Core.Utils;
using Doppelkopf.TestUtils;
using Xunit;

namespace Doppelkopf.Core.Tests.Games;

public class GameTests
{
  [Fact]
  public void TestInitialState()
  {
    var game = Configurations.Minikopf.CreateGameFactory(null).CreateGame(ByPlayer.Init(true));

    Assert.Null(game.CurrentTrick);
    Assert.Empty(game.CompleteTricks);
    Assert.Equal(GamePhase.Auction, game.Phase);
    Assert.Empty(game.Declarations);
  }

  [Fact]
  public void TestCompleteGame()
  {
    var cards = new ByPlayer<ImmutableArray<Card>>(
      [
        new Card(Suit.Hearts, Rank.Ten),
        new Card(Suit.Clubs, Rank.Queen),
        new Card(Suit.Hearts, Rank.Ace)
      ],
      [
        new Card(Suit.Clubs, Rank.Jack),
        new Card(Suit.Diamonds, Rank.Ace),
        new Card(Suit.Spades, Rank.Ace)
      ],
      [
        new Card(Suit.Hearts, Rank.Ten),
        new Card(Suit.Clubs, Rank.Queen),
        new Card(Suit.Hearts, Rank.Nine)
      ],
      [
        new Card(Suit.Diamonds, Rank.Ace),
        new Card(Suit.Hearts, Rank.Ace),
        new Card(Suit.Hearts, Rank.King)
      ]
    );
    var game = new Game(cards, new ByPlayer<bool>(true, false, false, false), Configurations.Minikopf);

    game.Play(Player.One, Declaration.Fine);
    game.Play(Player.Two, Declaration.Fine);
    game.Play(Player.Three, Declaration.Fine);
    game.Play(Player.Four, Declaration.Fine);

    Assert.NotNull(game.CurrentTrick);
    Assert.Equal(GamePhase.TrickTaking, game.Phase);
    var expectedAuctionResult = new AuctionResult(null, null, false);
    Assert.Equal(expectedAuctionResult, game.AuctionResult);

    // trick 1
    game.PlayCard(Player.One, new(Suit.Hearts, Rank.Ace));
    game.PlayCard(Player.Two, new(Suit.Diamonds, Rank.Ace));
    game.PlayCard(Player.Three, new(Suit.Hearts, Rank.Nine));
    game.PlayCard(Player.Four, new Card(Suit.Hearts, Rank.King));

    var trick1 = Assert.Single(game.CompleteTricks);
    Assert.Equal(Player.Two, trick1.Winner);

    // trick 2
    game.PlayCard(Player.Two, new(Suit.Spades, Rank.Ace));
    game.PlayCard(Player.Three, new(Suit.Clubs, Rank.Queen));
    game.PlayCard(Player.Four, new(Suit.Hearts, Rank.Ace));
    game.PlayCard(Player.One, new(Suit.Clubs, Rank.Queen));

    Assert.Equal(2, game.CompleteTricks.Count);
    var trick2 = game.CompleteTricks[^1];
    Assert.Equal(Player.Three, trick2.Winner);

    // trick 3
    game.PlayCard(Player.Three, new(Suit.Hearts, Rank.Ten));
    game.PlayCard(Player.Four, new(Suit.Diamonds, Rank.Ace));
    game.PlayCard(Player.One, new(Suit.Hearts, Rank.Ten));
    game.PlayCard(Player.Two, new(Suit.Clubs, Rank.Jack));
    Assert.Equal(3, game.CompleteTricks.Count);
    var trick3 = game.CompleteTricks[^1];
    Assert.Equal(Player.One, trick3.Winner);

    Assert.Equal(GamePhase.Finished, game.Phase);
    Assert.Null(game.CurrentTrick);
  }
}
