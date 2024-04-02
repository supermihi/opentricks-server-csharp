using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Xunit;

namespace Doppelkopf.Core.Tests.Scoring;

public class GameEvaluatorTests
{
  [Fact]
  public void NormalGame121To119()
  {
    var result = Evaluate(121, 119);
    Assert.Equal((Party.Re, 1), result);
  }

  [Fact]
  public void NormalGame120To120()
  {
    var result = Evaluate(120, 120);
    Assert.Equal((Party.Contra, 1), result);
  }

  [Fact]
  public void SoloGame121To119()
  {
    var result = Evaluate(121, 119);
    Assert.Equal((Party.Re, 1), result);
  }

  [Fact]
  public void SoloGame120To120()
  {
    var result = Evaluate(120, 120);
    Assert.Equal((Party.Contra, 1), result);
  }

  [Fact]
  public void NormalGameNoWinner()
  {
    var result = Evaluate(121, 119, Bid.No90, Bid.No90);
    Assert.Equal((null, 1), result);
  }

  [Fact]
  public void NormalGame121To119WithRe()
  {
    var result = Evaluate(121, 119, Bid.Re);
    Assert.Equal((Party.Re, 3), result);
  }

  [Fact]
  public void NormalGame120To120WithContra()
  {
    var result = Evaluate(120, 120, null, Bid.Contra);
    Assert.Equal((Party.Re, 3), result);
  }

  [Fact]
  public void NormalGame119To121WithContra()
  {
    var result = Evaluate(119, 121, null, Bid.Contra);
    Assert.Equal((Party.Contra, 3), result);
  }

  private static (Party? winner, int score) Evaluate(int rePoints, int contraPoints,
    Bid? maxReBid = null,
    Bid? maxContraBid = null,
    bool reSchwarz = false, bool contraSchwarz = false)
  {
    var reTotals = new PartyTotals(rePoints, reSchwarz, maxReBid);
    var contraTotals = new PartyTotals(contraPoints, contraSchwarz, maxContraBid);
    var totals = ByParty.New(reTotals, contraTotals);
    var winner = DdkvEvaluator.GetWinner(totals);
    var baseScore = DdkvEvaluator.GetBaseScore(totals, winner);
    return (winner, baseScore.Sum(s => s.Amount));
  }
}
