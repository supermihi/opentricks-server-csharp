using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring;

public sealed record GameEvaluation(
  Party? Winner,
  ByPlayer<Party> Parties,
  int BaseScore,
  IReadOnlyList<ExtraPoint> ExtraPoints)
{
  public ByParty<int> ScoreByParty()
  {
    var baseScore = Winner is null
      ? ByParty.Both(0)
      : ByParty.Init(p => p == Winner ? BaseScore : -BaseScore);
    var extraPoints = ByParty.Init(party => ExtraPoints.Count(point => point.Party == party));
    return baseScore.Add(extraPoints);
  }

  public ByPlayer<int> ScoreByPlayer()
  {
    var byParty = ScoreByParty();
    var soloist = Parties.Soloist();
    return ByPlayer.Init(player => byParty[Parties[player]] * (player == soloist ? 3 : 1));
  }
}
