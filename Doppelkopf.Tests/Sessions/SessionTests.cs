using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Conf;
using Doppelkopf.Contracts;
using Doppelkopf.Games;
using Doppelkopf.Sessions;
using Moq;

namespace Doppelkopf.Tests.Sessions;

public class SessionTests
{
  [Theory]
  [InlineData(12)]
  [InlineData(13)]
  public void IsFinishedOnlyAfterNumberOfGames(int gamesPlayed)
  {
    var sessionConfig = new SessionConfiguration(13, false);
    var finishedGame = new FinishedGame(
      Mock.Of<IContract>(),
      PartyData.NothingClarified,
      ByPlayer.Init(new Seat(1)),
      null,
      ByPlayer.Init(1));
    var session = new Session(
      Configuration.Default(EldersMode.FirstWins, sessionConfig, Decks.WithNines),
      4,
      new GameHistory(Enumerable.Repeat(finishedGame, gamesPlayed).ToImmutableArray()),
      null,
      ByPlayer.Init(new Seat(1)));
    Assert.Equal(gamesPlayed == 13, session.IsFinished);
  }
}
