using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;
using Doppelkopf.Games;
using Doppelkopf.Tricks;
using Doppelkopf.Utils;

namespace Doppelkopf.Tests.Tricks;

public class TrickTakingTests
{
  private static readonly ByPlayer<IImmutableList<Card>> DefaultCards =
      new(
        ImmutableList.Create<Card>(
          new(Suit.Hearts, Rank.Nine),
          new(Suit.Hearts, Rank.King),
          new(Suit.Hearts, Rank.Ace),
          new(Suit.Hearts, Rank.Ace),
          new(Suit.Spades, Rank.Ten),
          new(Suit.Spades, Rank.Ace),
          new(Suit.Diamonds, Rank.Nine),
          new(Suit.Diamonds, Rank.Jack),
          new(Suit.Diamonds, Rank.Jack),
          new(Suit.Clubs, Rank.Jack),
          new(Suit.Spades, Rank.Queen),
          new(Suit.Diamonds, Rank.Ten)
        ),
        ImmutableList.Create<Card>(
          new(Suit.Hearts, Rank.Nine),
          new(Suit.Spades, Rank.Nine),
          new(Suit.Spades, Rank.Nine),
          new(Suit.Spades, Rank.Ace),
          new(Suit.Clubs, Rank.Nine),
          new(Suit.Clubs, Rank.Ten),
          new(Suit.Clubs, Rank.Ten),
          new(Suit.Diamonds, Rank.King),
          new(Suit.Diamonds, Rank.King),
          new(Suit.Diamonds, Rank.Ace),
          new(Suit.Hearts, Rank.Jack),
          new(Suit.Clubs, Rank.Queen)
        ),
        ImmutableList.Create<Card>(
          new(Suit.Spades, Rank.King),
          new(Suit.Clubs, Rank.King),
          new(Suit.Clubs, Rank.King),
          new(Suit.Clubs, Rank.Ace),
          new(Suit.Diamonds, Rank.Ace),
          new(Suit.Diamonds, Rank.Jack),
          new(Suit.Diamonds, Rank.Jack),
          new(Suit.Spades, Rank.Jack),
          new(Suit.Diamonds, Rank.Queen),
          new(Suit.Hearts, Rank.Queen),
          new(Suit.Hearts, Rank.Queen),
          new(Suit.Clubs, Rank.Queen)
        ),
        ImmutableList.Create<Card>(
          new(Suit.Hearts, Rank.King),
          new(Suit.Spades, Rank.King),
          new(Suit.Spades, Rank.Ten),
          new(Suit.Clubs, Rank.Nine),
          new(Suit.Clubs, Rank.Ace),
          new(Suit.Diamonds, Rank.Nine),
          new(Suit.Hearts, Rank.Jack),
          new(Suit.Spades, Rank.Jack),
          new(Suit.Clubs, Rank.Jack),
          new(Suit.Diamonds, Rank.Queen),
          new(Suit.Spades, Rank.Queen),
          new(Suit.Diamonds, Rank.Ten)
        )
      );

  private static TrickTaking CreateInitialTrickTaking()
  {
    var contract = new NormalGame("normal");
    return TrickTaking.Initial(
      contract,
      new TrickConfiguration(EldersMode.FirstWins),
      DefaultCards
    );
  }

  [Fact]
  public void Initial()
  {
    var trickTaking = CreateInitialTrickTaking();
    Assert.NotNull(trickTaking.CurrentTrick);
    Assert.Empty(trickTaking.CompleteTricks);

    var initialTrick = trickTaking.CurrentTrick!;
    Assert.Equal(Player.Player1, initialTrick.Leader);
    Assert.Empty(initialTrick.Cards);
  }

  [Theory]
  [InlineData(Player.Player2)]
  [InlineData(Player.Player3)]
  [InlineData(Player.Player4)]
  public void PlayCardThrowsWhenNotPlayersTurn(Player player)
  {
    var trickTaking = CreateInitialTrickTaking();
    Assert.Equal(Player.Player1, trickTaking.CurrentTrick!.Turn);
    var exception = Assert.Throws<OtherPlayersTurnException>(
      () => trickTaking.PlayCard(player, trickTaking.Cards[player].First())
    );
    Assert.Equal(Player.Player1, exception.Player);
  }

  [Fact]
  public void PlayCardThrowsWhenDoesNotHaveCard()
  {
    var trickTaking = CreateInitialTrickTaking();
    var cardNotInPlayersHand = new Card(Suit.Clubs, Rank.Ace);
    var exception = Assert.Throws<InputException>(
      () => trickTaking.PlayCard(Player.Player1, cardNotInPlayersHand)
    );
    Assert.Equal(Errors.TrickTaking.DoNotHaveCard, exception);
  }

  [Fact]
  public void PlayCardThrowsWhenCardNotValid()
  {
    var trickTaking = CreateInitialTrickTaking();
    trickTaking = trickTaking.PlayCard(Player.Player1, new(Suit.Spades, Rank.Ace)).result;
    trickTaking = trickTaking.PlayCard(Player.Player2, new(Suit.Spades, Rank.Nine)).result;
    var exception = Assert.Throws<InputException>(
      () => trickTaking.PlayCard(Player.Player3, new(Suit.Clubs, Rank.Queen))
    );
    Assert.Equal(Errors.TrickTaking.InvalidCard, exception);
  }

  [Fact]
  public void PlayCardSuccess()
  {
    var before = CreateInitialTrickTaking();
    var playedCard = new Card(Suit.Spades, Rank.Ace);
    var (result, finishedTrick) = before.PlayCard(Player.Player1, playedCard);

    Assert.False(finishedTrick);
    var cardsBefore = before.Cards[Player.Player1];
    var cardsAfter = result.Cards[Player.Player1];
    Assert.Equal(cardsBefore.Count - 1, cardsAfter.Count);
    Assert.Equal(cardsBefore.Remove(playedCard), cardsAfter);

    Assert.Equal(before.Cards[Player.Player2], result.Cards[Player.Player2]);
    Assert.Equal(before.Cards[Player.Player3], result.Cards[Player.Player3]);
    Assert.Equal(before.Cards[Player.Player4], result.Cards[Player.Player4]);
  }
}
