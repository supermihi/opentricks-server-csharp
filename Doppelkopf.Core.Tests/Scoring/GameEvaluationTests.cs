using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Utils;
using Xunit;

namespace Doppelkopf.Core.Tests.Scoring;

public class GameEvaluationTests
{
  [Theory]
  [InlineData(Party.Re)]
  [InlineData(Party.Contra)]
  public void ByPartyScoreNormalGame(Party winner)
  {
    var parties = new ByPlayer<Party>(Party.Re, Party.Re, Party.Contra, Party.Contra);
    var evaluation = new GameEvaluation(
      winner,
      parties,
      [
        new Score("won", winner),
        new Score("no 90", winner),
        new Score("announced no 90", winner),
        new Score("fox", winner.Other())
      ]);
    var scoreByParty = evaluation.ScoreByParty();
    Assert.Equal(2, scoreByParty[winner]);
    Assert.Equal(-2, scoreByParty[winner.Other()]);
  }

  [Fact]
  public void ByPartyScoreReWinsButScoresNegativeDueToExtras()
  {
    var parties = new ByPlayer<Party>(Party.Re, Party.Re, Party.Contra, Party.Contra);
    var evaluation = new GameEvaluation(
      Party.Re,
      parties,
      [
        new Score("won", Party.Re),
        new Score("fox", Party.Contra),
        new Score("doppelkopf", Party.Contra)
      ]);
    var scoreByParty = evaluation.ScoreByParty();
    Assert.Equal(1, scoreByParty[Party.Contra]);
    Assert.Equal(-1, scoreByParty[Party.Re]);
  }

  [Fact]
  public void ByPartyScoreSolo()
  {
    var parties = new ByPlayer<Party>(Party.Re, Party.Contra, Party.Contra, Party.Contra);
    var evaluation = new GameEvaluation(
      Party.Contra,
      parties,
      [
        new Score("won", Party.Re),
        new Score("no 90", Party.Re)
      ]);
    var scoreByParty = evaluation.ScoreByParty();
    Assert.Equal(2, scoreByParty[Party.Re]);
    Assert.Equal(-2, scoreByParty[Party.Contra]);

    var scoreByPlayer = evaluation.ScoreByPlayer();
    Assert.Equal(6, scoreByPlayer[Player.One]);
    Assert.Equal(-2, scoreByPlayer[Player.Two]);
    Assert.Equal(-2, scoreByPlayer[Player.Three]);
    Assert.Equal(-2, scoreByPlayer[Player.Four]);
  }
}
