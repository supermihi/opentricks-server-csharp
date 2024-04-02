using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring;

public sealed record GameEvaluation(
  Party? Winner,
  ByPlayer<Party> Parties,
  IReadOnlyList<Score> Scores)
{
  public ByParty<int> ScoreByParty()
  {
    var re = Scores.Sum(s => s.Party == Party.Re ? s.Amount : -s.Amount);
    return ByParty.New(re, -re);
  }

  public ByPlayer<int> ScoreByPlayer()
  {
    var byParty = ScoreByParty();
    var soloist = Parties.Soloist();
    return ByPlayer.Init(player => byParty[Parties[player]] * (player == soloist ? 3 : 1));
  }
}
