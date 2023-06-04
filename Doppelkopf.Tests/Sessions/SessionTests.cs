using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Games;
using Doppelkopf.Sessions;
using Doppelkopf.Tricks;

namespace Doppelkopf.Tests.Sessions;

public class SessionTests
{
  [Theory]
  [InlineData(12)]
  [InlineData(13)]
  public void IsFinishedOnlyAfterNumberOfGames(int gamesPlayed)
  {
    var sessionConfig = new SessionConfiguration(13, false);
    var completeGame = new CompleteGame(
      GameFactory.InitialMinikopfGame(),
      ByPlayer.Init(new Seat(1)),
      ByPlayer.Init(1));
    var session = new Session(
      Configuration.Default(EldersMode.FirstWins, sessionConfig, new RandomCardProvider(Decks.WithNines)),
      4,
      new Finishedgames(Enumerable.Repeat(completeGame, gamesPlayed).ToImmutableArray()),
      GameFactory.InitialMinikopfGame(),
      ByPlayer.Init(new Seat(1)));
    Assert.Equal(gamesPlayed == 13, session.IsFinished);
  }
}
