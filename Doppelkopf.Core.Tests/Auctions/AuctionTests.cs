using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Auctions.Impl;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;
using Doppelkopf.TestUtils;
using Moq;
using Xunit;

namespace Doppelkopf.Core.Tests.Auctions;

public class AuctionTests
{
    private static IDeclarableContract AllowingContract =>
        Mock.Of<IDeclarableContract>(
            c => c.IsAllowed(It.IsAny<IEnumerable<Card>>()) == true,
            MockBehavior.Strict
        );

    private static IByPlayer<bool> NoCompulsorySolos =>
        Mock.Of<IByPlayer<bool>>(MockBehavior.Loose);
    private static ICardsByPlayer MockCards => Mock.Of<ICardsByPlayer>();

    private static IDeclarableContract DeclaredSolo =>
        Mock.Of<IDeclarableContract>(c => c.Type == DeclaredContractType.Solo);

    [Fact]
    public void PlayerOneHasFirstTurn()
    {
        var auction = new Auction(MockCards, NoCompulsorySolos);
        Assert.Equal(Player.One, auction.Turn);
    }

    [Theory]
    [InlineData(Player.Two)]
    [InlineData(Player.Three)]
    [InlineData(Player.Four)]
    public void ThrowsIfNotTurn(Player player)
    {
        var auction = new Auction(MockCards, NoCompulsorySolos);
        Asserts.ThrowsErrorCode(
            ErrorCodes.NotYourTurn,
            () => auction.DeclareReservation(player, AllowingContract)
        );
    }

    [Fact]
    public void ThrowsIfContractNotAllowed()
    {
        var contract = Mock.Of<IDeclarableContract>(
            c => c.IsAllowed(It.IsAny<IEnumerable<Card>>()) == false,
            MockBehavior.Strict
        );
        var auction = new Auction(MockCards, NoCompulsorySolos);
        Asserts.ThrowsErrorCode(
            ErrorCodes.ContractNotAllowed,
            () => auction.DeclareReservation(auction.Turn!.Value, contract)
        );
    }

    [Fact]
    public void UpdatesTurn()
    {
        var auction = new Auction(MockCards, NoCompulsorySolos);
        auction.DeclareReservation(Player.One, AllowingContract);
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
        var auction = new Auction(MockCards, NoCompulsorySolos, state);
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
        var auction = new Auction(MockCards, NoCompulsorySolos, state);
        Assert.Null(auction.Evaluate());
    }

    [Fact]
    public void EvaluatesNormalGameIfAllOk()
    {
        var state = new InTurns<Declaration>(Player.One, Enumerable.Repeat(Declaration.Ok, 4));
        var auction = new Auction(MockCards, NoCompulsorySolos, state);
        Assert.Equal(AuctionResult.NormalGame, auction.Evaluate());
    }

    [Fact]
    public void FirstVoluntarySoloWins()
    {
        var state = new InTurns<Declaration>(
            Player.One,
            Declaration.Ok,
            Declaration.FromContract(DeclaredSolo),
            Declaration.Ok,
            Declaration.FromContract(DeclaredSolo)
        );
        var auction = new Auction(MockCards, NoCompulsorySolos, state);
        var result = auction.Evaluate();
        Assert.NotNull(result?.Contract);
        Assert.Equal(Player.Two, result.Contract.Declarer);
    }

    [Fact]
    public void FirstCompulsorySoloWins()
    {
        var state = new InTurns<Declaration>(
            Player.One,
            Declaration.Ok,
            Declaration.FromContract(DeclaredSolo),
            Declaration.Ok,
            Declaration.FromContract(DeclaredSolo)
        );
        var compulsorySolos = new ByPlayer<bool>(false, false, true, true);
        var auction = new Auction(MockCards, compulsorySolos, state);
        var result = auction.Evaluate();
        Assert.NotNull(result?.Contract);
        Assert.Equal(Player.Four, result.Contract.Declarer);
    }
}
