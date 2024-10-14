using System.Collections.Immutable;
using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Auctions.Impl;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;
using Doppelkopf.TestUtils;
using Moq;
using Xunit;

namespace Doppelkopf.Core.Tests.Auctions;

public class AuctionTests
{
  private static readonly IHold AllowingHold =
    Mock.Of<IHold>(
      c => c.IsAllowed(It.IsAny<IEnumerable<Card>>()) == true,
      MockBehavior.Strict
    );

  private static ByPlayer<bool> NoCompulsorySolos => ByPlayer.Init(false);
  private static ByPlayer<ImmutableArray<Card>> MockCards => ByPlayer.Init(ImmutableArray<Card>.Empty);
  private static IReadOnlyList<IHold> NoContracts => Array.Empty<IHold>();

  private static IHold DeclarableSolo
  {
    get
    {
      var declarableSolo = new Mock<IHold>();
      declarableSolo.Setup(s => s.IsSolo).Returns(true);
      declarableSolo.Setup(s => s.Priority).Returns(new DeclarationPriority(2, 3));
      declarableSolo.Setup(s => s.IsAllowed(It.IsAny<IEnumerable<Card>>())).Returns(true);
      return declarableSolo.Object;
    }
  }

  [Fact]
  public void PlayerOneHasFirstTurn()
  {
    var auction = new Auction(MockCards, NoContracts, NoCompulsorySolos);
    Assert.Equal(Player.One, auction.Turn);
  }

  [Theory]
  [InlineData(Player.Two)]
  [InlineData(Player.Three)]
  [InlineData(Player.Four)]
  public void DeclareReservationThrowsIfNotTurn(Player player)
  {
    var auction = new Auction(
      MockCards,
      new[] { AllowingHold },
      NoCompulsorySolos);
    Asserts.ThrowsErrorCode(
      ErrorCodes.NotYourTurn,
      () => auction.DeclareReservation(player, AllowingHold)
    );
  }

  [Fact]
  public void DeclareReservationThrowsIfContractNotAllowed()
  {
    var contract = Mock.Of<IHold>(
      c => c.IsAllowed(It.IsAny<IEnumerable<Card>>()) == false,
      MockBehavior.Strict
    );
    var auction = new Auction(
      MockCards,
      new[] { contract },
      NoCompulsorySolos);
    Asserts.ThrowsErrorCode(
      ErrorCodes.ContractNotAllowed,
      () => auction.DeclareReservation(auction.Turn!.Value, contract)
    );
  }

  [Fact]
  public void DeclareReservationThrowsIfContractDoesNotExist()
  {
    var auction = new Auction(MockCards, [], NoCompulsorySolos);
    Asserts.ThrowsErrorCode(ErrorCodes.ContractNotAvailable,
    () => auction.DeclareReservation(auction.Turn!.Value, AllowingHold));
  }

  [Fact]
  public void DeclareUpdatesTurn()
  {
    var auction = new Auction(
      MockCards,
      new[] { AllowingHold },
      NoCompulsorySolos);
    auction.DeclareReservation(Player.One, AllowingHold);
    Assert.Equal(Player.Two, auction.Turn);
    auction.DeclareOk(Player.Two);
    Assert.Equal(Player.Three, auction.Turn);
    auction.DeclareOk(Player.Three);
    Assert.Equal(Player.Four, auction.Turn);
    auction.DeclareOk(Player.Four);
    Assert.Null(auction.Turn);
  }

  [Fact]
  public void DeclareThrowsIfFinished()
  {
    var state = new InTurns<Declaration>(
      Player.One,
      Declaration.Ok,
      Declaration.Ok,
      Declaration.Ok,
      Declaration.Ok
    );
    var auction = new Auction(MockCards, NoContracts, NoCompulsorySolos, state);
    Assert.Null(auction.Turn);
    Asserts.ThrowsErrorCode(ErrorCodes.InvalidPhase, () => auction.DeclareOk(Player.One));
  }

  [Theory]
  [InlineData(0)]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void EvaluatesNullIfNotFinished(int numPlayersDeclared)
  {
    var state = new InTurns<Declaration>(
      Player.One,
      Enumerable.Repeat(Declaration.Ok, numPlayersDeclared)
    );
    var auction = new Auction(MockCards, NoContracts, NoCompulsorySolos, state);
    Assert.Null(auction.Evaluate());
  }

  [Fact]
  public void EvaluatesNormalGameIfAllOk()
  {
    var state = new InTurns<Declaration>(Player.One, Enumerable.Repeat(Declaration.Ok, 4));
    var auction = new Auction(MockCards, NoContracts, NoCompulsorySolos, state);
    var result = auction.Evaluate();

    var expected = new AuctionResult(null, null, false);
    Assert.Equal(expected, result);
  }

  [Fact]
  public void FirstVoluntarySoloWins()
  {
    var state = new InTurns<Declaration>(
      Player.One,
      Declaration.Ok,
      Declaration.FromHold(DeclarableSolo),
      Declaration.Ok,
      Declaration.FromHold(DeclarableSolo)
    );
    var auction = new Auction(MockCards, NoContracts, NoCompulsorySolos, state);
    var result = auction.Evaluate();
    Assert.NotNull(result);
    Assert.Equal(Player.Two, result.Declarer);
  }

  [Fact]
  public void FirstCompulsorySoloWins()
  {
    var state = new InTurns<Declaration>(
      Player.One,
      Declaration.Ok,
      Declaration.FromHold(DeclarableSolo),
      Declaration.Ok,
      Declaration.FromHold(DeclarableSolo)
    );
    var compulsorySolos = new ByPlayer<bool>(false, false, true, true);
    var auction = new Auction(MockCards, NoContracts, compulsorySolos, state);
    var result = auction.Evaluate();
    Assert.NotNull(result);
    Assert.Equal(Player.Four, result.Declarer);
  }
}
