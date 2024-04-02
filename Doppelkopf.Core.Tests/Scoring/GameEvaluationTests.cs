using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Utils;
using Xunit;

namespace Doppelkopf.Core.Tests.Scoring;

public class GameEvaluationTests
{
  [Theory]
  [InlineData(Party.Re)]
  [InlineData(Party.Contra)]
  public void ByPartyScoreNormalGameNoExtra(Party winner)
  {
    var parties = new ByPlayer<Party>(Party.Re, Party.Re, Party.Contra, Party.Contra);
    var evaluation = new GameEvaluation(winner, parties, 3, []);
    var scoreByParty = evaluation.ScoreByParty();
    Assert.Equal(3, scoreByParty[winner]);
    Assert.Equal(-3, scoreByParty[winner.Other()]);
  }

  [Fact]
  public void ByPartyScoreNormalGameWithExtras()
  {
    var parties = new ByPlayer<Party>(Party.Re, Party.Re, Party.Contra, Party.Contra);
    var evaluation = new GameEvaluation(
      Party.Contra,
      parties,
      3,
      [
        new ExtraPoint(ExtraPointIds.CaughtTheFox, Player.Four, Party.Contra, 1),
        new ExtraPoint(ExtraPointIds.Doppelkopf, Player.Two, Party.Re, 2),
        new ExtraPoint(ExtraPointIds.CaughtTheFox, Player.One, Party.Re, 12)
      ]);
    var scoreByParty = evaluation.ScoreByParty();
    Assert.Equal(2, scoreByParty[Party.Contra]);
    Assert.Equal(-2, scoreByParty[Party.Re]);
  }
}
