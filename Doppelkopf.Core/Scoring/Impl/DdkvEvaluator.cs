using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public class DdkvEvaluator : IEvaluator
{
  private static IEnumerable<IExtraScoreRule> ExtraScoreRules { get; } =
  [
    new CharlieMiller(),
    new Doppelkopf(),
    new AgainstTheElders(),
    new CaughtTheFox()
  ];

  public GameEvaluation Evaluate(IReadOnlyList<CompleteTrick> tricks, ByParty<Bid?> maxBids, ByPlayer<Party> parties)
  {
    var totals = PartyTotals.ComputeBoth(tricks, maxBids, parties);
    var winner = GetWinner(totals);
    var baseScore = GetBaseScore(totals, winner);
    var extraPoints = GetExtraPoints(tricks, parties, winner);

    return new GameEvaluation(winner, totals, parties, baseScore.Concat(extraPoints).ToList());
  }

  private static IEnumerable<Score> GetExtraPoints(IReadOnlyList<CompleteTrick> tricks, ByPlayer<Party> parties,
    Party? winner) =>
    ExtraScoreRules.SelectMany(r => r.Evaluate(tricks, parties, winner));

  public static Party? GetWinner(ByParty<PartyTotals> totals)
  {
    if (ReWins(totals))
    {
      return Party.Re;
    }
    if (ContraWins(totals))
    {
      return Party.Contra;
    }
    return null;
  }


  /// <summary>
  /// Implements section 7.1.2 of the DDKV's tournament rules.
  /// </summary>
  private static bool ReWins(ByParty<PartyTotals> totals)
  {
    var (re, contra) = totals;
    return (re.MaxBid, contra.MaxBid, re.Points) switch
    {
      (null, null, >= 121) => true, // 1.
      (Bid.Re, null or Bid.Contra, >= 121) => true, // 2. & 3.
      (null, Bid.Contra, >= 120) => true, // 4.
      (Bid.No90, _, >= 151) => true, // 5.
      (Bid.No60, _, >= 181) => true, // 5.
      (Bid.No30, _, >= 211) => true, // 5.
      (Bid.Schwarz, _, _) when contra.Schwarz => true, // 6.
      (null or Bid.Re, Bid.No90, >= 90) => true, // 7.
      (null or Bid.Re, Bid.No60, >= 60) => true, // 7.
      (null or Bid.Re, Bid.No30, >= 30) => true, // 7.
      (null or Bid.Re, Bid.Schwarz, _) when !re.Schwarz => true, // 8.
      _ => false
    };
  }

  /// <summary>
  /// Implements section 7.1.3
  /// </summary>
  /// <returns></returns>
  private static bool ContraWins(ByParty<PartyTotals> totals)
  {
    var (re, contra) = totals;
    return (contra.MaxBid, re.MaxBid, contra.Points) switch
    {
      (null, null or Bid.Re, >= 120) => true, // 1. & 2.
      (Bid.Contra, Bid.Re, >= 120) => true, // 3.
      (Bid.Contra, null, >= 121) => true, // 4.
      (Bid.No90, _, >= 151) => true, // 5.
      (Bid.No60, _, >= 181) => true, // 5.
      (Bid.No30, _, >= 211) => true, // 5.
      (Bid.Schwarz, _, _) when re.Schwarz => true, // 6.
      (null or Bid.Contra, Bid.No90, >= 90) => true, // 7.
      (null or Bid.Contra, Bid.No60, >= 60) => true, // 7.
      (null or Bid.Contra, Bid.No30, >= 30) => true, // 7.
      (null or Bid.Contra, Bid.Schwarz, _) when !contra.Schwarz => true, // 8.
      _ => false
    };
  }

  public static IEnumerable<Score> GetBaseScore(ByParty<PartyTotals> totals, Party? winner)
  {
    // 7.2.2 (a) 1. "Grundwert"
    var won = winner is { } w ? new[] { new Score(ScoreIds.Won, w) } : [];

    return won.Concat(BidScore(totals, winner))
      .Concat(ExtraScoreBasedOnOpponentPoints(totals, winner))
      .Concat(ExtraPointsForDistanceToRefusal(Party.Re, totals, winner))
      .Concat(ExtraPointsForDistanceToRefusal(Party.Contra, totals, winner));
  }

  /// <summary>
  /// DDKV rules, 7.2.2 (b) to (d)
  /// </summary>
  private static IEnumerable<Score> BidScore(ByParty<PartyTotals> totals, Party? winner)
  {
    if (winner is null)
    {
      yield break;
    }

    if (totals.Re.MaxBid is { } reBid)
    {
      yield return new Score(ScoreIds.AnnouncedRe, winner.Value, 2);
      if (reBid.IsOrImplies(Bid.No90))
      {
        yield return new Score(ScoreIds.AnnouncedNo90, winner.Value);
      }
      if (reBid.IsOrImplies(Bid.No60))
      {
        yield return new Score(ScoreIds.AnnouncedNo60, winner.Value);
      }
      if (reBid.IsOrImplies(Bid.No30))
      {
        yield return new Score(ScoreIds.AnnouncedNo30, winner.Value);
      }
      if (reBid == Bid.Schwarz)
      {
        yield return new Score(ScoreIds.AnnouncedSchwarz, winner.Value);
      }
    }
    if (totals.Contra.MaxBid is { } contraBid)
    {
      yield return new Score(ScoreIds.AnnouncedContra, winner.Value, 2);
      if (contraBid.IsOrImplies(Bid.No90))
      {
        yield return new Score(ScoreIds.AnnouncedNo90, winner.Value);
      }
      if (contraBid.IsOrImplies(Bid.No60))
      {
        yield return new Score(ScoreIds.AnnouncedNo60, winner.Value);
      }
      if (contraBid.IsOrImplies(Bid.No30))
      {
        yield return new Score(ScoreIds.AnnouncedNo30, winner.Value);
      }
      if (contraBid == Bid.Schwarz)
      {
        yield return new Score(ScoreIds.AnnouncedSchwarz, winner.Value);
      }
    }
  }

  /// <summary>
  /// DDKV rules, 7.2.2 (a) from line 2
  /// </summary>
  private static IEnumerable<Score> ExtraScoreBasedOnOpponentPoints(ByParty<PartyTotals> results, Party? winner)
  {
    foreach (var party in Enum.GetValues<Party>())
    {
      var scoringParty = winner ?? party; // TODO clarify if this is correct for winner == null
      if (results[party].Points < 90)
      {
        yield return new Score(ScoreIds.GotNo90, scoringParty);
      }
      if (results[party].Points < 60)
      {
        yield return new Score(ScoreIds.GotNo60, scoringParty);
      }
      if (results[party].Points < 30)
      {
        yield return new Score(ScoreIds.GotNo30, scoringParty);
      }
      if (results[party].Schwarz)
      {
        yield return new Score(ScoreIds.GotSchwarz, scoringParty);
      }
    }
  }


  /// <summary>
  /// DDKV rules, 7.2.2 (e) and (f)
  /// </summary>
  private static IEnumerable<Score> ExtraPointsForDistanceToRefusal(Party party, ByParty<PartyTotals> results,
    Party? winner)
  {
    if (results[party.Other()].MaxBid is not { } maxOpponentBid)
    {
      yield break;
    }
    var scoringParty = winner ?? party; // TODO clarify if this is correct for winner == null
    var ownPoints = results[party].Points;
    if (ownPoints >= 120 && maxOpponentBid.IsOrImplies(Bid.No90))
    {
      yield return new Score(ScoreIds.Got120AgainstNo90, scoringParty);
    }
    if (ownPoints >= 90 && maxOpponentBid.IsOrImplies(Bid.No60))
    {
      yield return new Score(ScoreIds.Got90AgainstNo60, scoringParty);
    }
    if (ownPoints >= 60 && maxOpponentBid.IsOrImplies(Bid.No30))
    {
      yield return new Score(ScoreIds.Got60AgainstNo30, scoringParty);
    }
    if (ownPoints >= 30 && maxOpponentBid == Bid.Schwarz)
    {
      yield return new Score(ScoreIds.Got30AgainstSchwarz, scoringParty);
    }
  }
}
