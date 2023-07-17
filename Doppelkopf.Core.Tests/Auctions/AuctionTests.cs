using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Auctions.Impl;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Utils;
using Doppelkopf.TestUtils;
using Moq;
using Xunit;

namespace Doppelkopf.Core.Tests.Auctions;

public class AuctionTests
{
  private static readonly IDeclarableContract allowingContract =
      Mock.Of<IDeclarableContract>(
        c => c.IsAllowed(It.IsAny<IEnumerable<Card>>()) == true,
        MockBehavior.Strict
      );

  private static IByPlayer<bool> NoCompulsorySolos => Mock.Of<IByPlayer<bool>>(MockBehavior.Loose);
  private static ICardsByPlayer MockCards => Mock.Of<ICardsByPlayer>();
  private static AvailableContracts MockContracts => new(Mock.Of<IContract>());

  private static IDeclarableContract DeclarableSolo
  {
    get
    {
      var declarableSolo = new Mock<IDeclarableContract>();
      declarableSolo.Setup(s => s.Type).Returns(ContractType.Solo);
      declarableSolo.Setup(s => s.IsAllowed(It.IsAny<IEnumerable<Card>>())).Returns(true);
      return declarableSolo.Object;
    }
  }

  [Fact]
  public void PlayerOneHasFirstTurn()
  {
    var auction = new Auction(MockCards, MockContracts, NoCompulsorySolos);
    Assert.Equal(Player.One, auction.Turn);
  }

  [Theory]
  [InlineData(Player.Two)]
  [InlineData(Player.Three)]
  [InlineData(Player.Four)]
  public void ThrowsIfNotTurn(Player player)
  {
    var auction = new Auction(
      MockCards,
      new AvailableContracts(Mock.Of<IContract>(), allowingContract),
      NoCompulsorySolos);
    Asserts.ThrowsErrorCode(
      ErrorCodes.NotYourTurn,
      () => auction.DeclareReservation(player, allowingContract)
    );
  }

  [Fact]
  public void ThrowsIfContractNotAllowed()
  {
    var contract = Mock.Of<IDeclarableContract>(
      c => c.IsAllowed(It.IsAny<IEnumerable<Card>>()) == false,
      MockBehavior.Strict
    );
    var auction = new Auction(
      MockCards,
      new AvailableContracts(Mock.Of<IContract>(), contract),
      NoCompulsorySolos);
    Asserts.ThrowsErrorCode(
      ErrorCodes.ContractNotAllowed,
      () => auction.DeclareReservation(auction.Turn!.Value, contract)
    );
  }

  [Fact]
  public void UpdatesTurn()
  {
    var auction = new Auction(
      MockCards,
      new AvailableContracts(Mock.Of<IContract>(), allowingContract),
      NoCompulsorySolos);
    auction.DeclareReservation(Player.One, allowingContract);
    Assert.Equal(Player.Two, auction.Turn);
    auction.DeclareOk(Player.Two);
    Assert.Equal(Player.Three, auction.Turn);
    auction.DeclareOk(Player.Three);
    Assert.Equal(Player.Four, auction.Turn);
    auction.DeclareOk(Player.Four);
    Assert.Null(auction.Turn);
  }

  [Fact]
  public void ThrowsIfFinished()
  {
    var state = new InTurns<Declaration>(
      Player.One,
      Declaration.Ok,
      Declaration.Ok,
      Declaration.Ok,
      Declaration.Ok
    );
    var auction = new Auction(MockCards, MockContracts, NoCompulsorySolos, state);
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
    var auction = new Auction(MockCards, MockContracts, NoCompulsorySolos, state);
    Assert.Null(auction.Evaluate());
  }

  [Fact]
  public void EvaluatesNormalGameIfAllOk()
  {
    var state = new InTurns<Declaration>(Player.One, Enumerable.Repeat(Declaration.Ok, 4));
    var normalGame = Mock.Of<IContract>();
    var contracts = new AvailableContracts(normalGame);
    var auction = new Auction(MockCards, contracts, NoCompulsorySolos, state);
    var result = auction.Evaluate();

    var expected = new AuctionResult(normalGame, null, null);
    Assert.Equal(expected, result);
  }

  [Fact]
  public void FirstVoluntarySoloWins()
  {
    var state = new InTurns<Declaration>(
      Player.One,
      Declaration.Ok,
      Declaration.FromContract(DeclarableSolo),
      Declaration.Ok,
      Declaration.FromContract(DeclarableSolo)
    );
    var auction = new Auction(MockCards, MockContracts, NoCompulsorySolos, state);
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
      Declaration.FromContract(DeclarableSolo),
      Declaration.Ok,
      Declaration.FromContract(DeclarableSolo)
    );
    var compulsorySolos = new ByPlayer<bool>(false, false, true, true);
    var auction = new Auction(MockCards, MockContracts, compulsorySolos, state);
    var result = auction.Evaluate();
    Assert.NotNull(result);
    Assert.Equal(Player.Four, result.Declarer);
  }
}
