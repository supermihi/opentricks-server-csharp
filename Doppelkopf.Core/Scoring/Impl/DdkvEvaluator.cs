using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public class DdkvEvaluator : IEvaluator
{
  private static IEnumerable<IExtraPointRule> ExtraPointRules { get; } =
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
    var baseScore = GetBaseScore(totals, winner == null);
    var extraPoints = GetExtraPoints(tricks, parties, winner);

    return new GameEvaluation(winner, parties, baseScore, extraPoints.ToList());
  }

  private static IEnumerable<ExtraPoint> GetExtraPoints(IReadOnlyList<CompleteTrick> tricks, ByPlayer<Party> parties,
    Party? winner) =>
    ExtraPointRules.SelectMany(r => r.Evaluate(tricks, parties, winner));

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
      (Bid.NoNinety, _, >= 151) => true, // 5.
      (Bid.NoSixty, _, >= 181) => true, // 5.
      (Bid.NoThirty, _, >= 211) => true, // 5.
      (Bid.Schwarz, _, _) when contra.Schwarz => true, // 6.
      (null or Bid.Re, Bid.NoNinety, >= 90) => true, // 7.
      (null or Bid.Re, Bid.NoSixty, >= 60) => true, // 7.
      (null or Bid.Re, Bid.NoThirty, >= 30) => true, // 7.
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
      (Bid.NoNinety, _, >= 151) => true, // 5.
      (Bid.NoSixty, _, >= 181) => true, // 5.
      (Bid.NoThirty, _, >= 211) => true, // 5.
      (Bid.Schwarz, _, _) when re.Schwarz => true, // 6.
      (null or Bid.Contra, Bid.NoNinety, >= 90) => true, // 7.
      (null or Bid.Contra, Bid.NoSixty, >= 60) => true, // 7.
      (null or Bid.Contra, Bid.NoThirty, >= 30) => true, // 7.
      (null or Bid.Contra, Bid.Schwarz, _) when !contra.Schwarz => true, // 8.
      _ => false
    };
  }

  public static int GetBaseScore(ByParty<PartyTotals> totals, bool noWinner) =>
    1 // 7.2.2 (a) 1. "Grundwert"
    + (noWinner ? 0 : BidScore(totals))
    + ExtraScoreBasedOnOpponentPoints(totals)
    + ExtraPointsForDistanceToRefusal(Party.Re, totals)
    + ExtraPointsForDistanceToRefusal(Party.Contra, totals);

  /// <summary>
  /// DDKV rules, 7.2.2 (b) to (d)
  /// </summary>
  private static int BidScore(ByParty<PartyTotals> totals) =>
    (totals.Re.MaxBid?.ExtraScore() ?? 0) // 7.2.2 (b) 1. and (c)
    + (totals.Contra.MaxBid?.ExtraScore() ?? 0); // 7.2.2 (b) 1. and (d)

  /// <summary>
  /// DDKV rules, 7.2.2 (a) from line 2
  /// </summary>
  private static int ExtraScoreBasedOnOpponentPoints(ByParty<PartyTotals> results)
  {
    var minPoints = Math.Min(results.Re.Points, results.Contra.Points);
    return (minPoints < 90 ? 1 : 0) // 7.2.2 (a) 2.
      + (minPoints < 60 ? 1 : 0) // 7.2.2 (a) 3.
      + (minPoints < 30 ? 1 : 0) // 7.2.2 (a) 4.
      + (results.Any(r => r.Schwarz) ? 1 : 0); // 7.2.2 (a) 5.
  }


  /// <summary>
  /// DDKV rules, 7.2.2 (e) and (f)
  /// </summary>
  private static int ExtraPointsForDistanceToRefusal(Party party, ByParty<PartyTotals> results)
  {
    var maxOpponentBid = results.GetValue(party.Other()).MaxBid;
    return (maxOpponentBid, results.GetValue(party).Points) switch
    {
      (Bid.NoNinety, >= 120) => 1,
      (Bid.NoSixty, >= 90) => 2,
      (Bid.NoThirty, >= 60) => 3,
      (Bid.Schwarz, >= 30) => 4,
      _ => 0
    };
  }
}
