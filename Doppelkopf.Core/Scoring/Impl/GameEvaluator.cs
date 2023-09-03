using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

internal static class Evaluation
{
  public static (Party? winner, ByPlayer<int> score) Evaluate(IReadOnlyCollection<CompleteTrick> tricks,
    IByPlayer<Party> parties, ByParty<Bid?> maxBids)
  {
    var points = ByParty.Init(party => tricks.Where(t => parties[t.Winner] == party).Sum(t => t.Points()));
    var schwarz = ByParty.Init(party => tricks.All(t => parties[t.Winner] != party));

    return Evaluate(parties, points, maxBids, schwarz);
  }

  public static (Party? winner, ByPlayer<int> score) Evaluate(IByPlayer<Party> parties, ByParty<int> points,
    ByParty<Bid?> maxBids, ByParty<bool> schwarz)
  {
    var isTwoVsTwo = parties.Count(p => p == Party.Re) == 2;
    var (winnerOrNull, baseScore) = WinnerAndBaseScore(points, maxBids, schwarz, isTwoVsTwo);

    if (winnerOrNull is not { } winner)
    {
      return (null, ByPlayer.Init(0)); // TODO check what should happen in this case
    }
    var scoreByPlayer = ByPlayer.Init(
      player =>
      {
        var sign = parties[player] == winner ? 1 : -1;
        var multiplier = parties.Count(p => p == parties[player]) == 1 ? 3 : 1;
        return baseScore * sign * multiplier;
      });
    return (winner, scoreByPlayer);
  }

  public static (Party? winner, int score) WinnerAndBaseScore(ByParty<int> points, ByParty<Bid?> maxBids,
    ByParty<bool> schwarz, bool isTwoVsTwo)
  {
    var results = new ByParty<Result>(
      new(points.Re, maxBids.Re, schwarz.Re),
      new(points.Contra, maxBids.Contra, schwarz.Contra));
    var winner = WinningParty(results);
    if (winner is { } party)
    {
      return (winner, BaseScore(party, results, isTwoVsTwo));
    }
    return (null, 0);
  }

  private static Party? WinningParty(ByParty<Result> results)
  {
    if (ReWins(results))
    {
      return Party.Re;
    }
    if (ContraWins(results))
    {
      return Party.Contra;
    }
    return null;
  }

  private readonly record struct Result(int Points, Bid? MaxBid, bool Schwarz);

  /// <summary>
  /// Implements section 7.1.2 of the DDKV's tournament rules.
  /// </summary>
  private static bool ReWins(ByParty<Result> results)
  {
    var (re, contra) = results;
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
  private static bool ContraWins(ByParty<Result> results)
  {
    var (re, contra) = results;
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

  private static int BaseScore(Party winner, ByParty<Result> results, bool isTwoVsTwo)
  {
    return 1 // 7.2.2 (a) 1.
        + (winner is Party.Contra && isTwoVsTwo ? 1 : 0) // 7.2.3 (1) "gegen die Alten"
        + BidScore(results)
        + ExtraScoreBasedOnOpponentPoints(results)
        + ExtraPointsForDistanceToRefusal(Party.Re, results)
        + ExtraPointsForDistanceToRefusal(Party.Contra, results);
  }

  /// <summary>
  /// DDKV rules, 7.2.2 (b)
  /// </summary>
  private static int BidScore(ByParty<Result> results)
  {
    return (results.Re.MaxBid?.ExtraScore() ?? 0) // 7.2.2 (b) 1.
        + (results.Contra.MaxBid?.ExtraScore() ?? 0); // 7.2.2 (b) 2.
  }

  private static int ExtraScoreBasedOnOpponentPoints(ByParty<Result> results)
  {
    var minPoints = Math.Min(results.Re.Points, results.Contra.Points);
    return (minPoints < 90 ? 1 : 0) // 7.2.2 (a) 2.
        + (minPoints < 60 ? 1 : 0) // 7.2.2 (a) 3.
        + (minPoints < 30 ? 1 : 0) // 7.2.2 (a) 4.
        + (results.Any(r => r.Schwarz) ? 1 : 0); // 7.2.2 (a) 5.
  }

  private static int ExtraPointsForDistanceToRefusal(Party party, ByParty<Result> results)
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
