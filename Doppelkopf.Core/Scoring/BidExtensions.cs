namespace Doppelkopf.Core.Scoring;

public static class BidExtensions
{
  public static bool IsOrImplies(this Bid bid, Bid other) => other <= bid;

  public static int MaximumPlayedCards(this Bid bid) =>
    bid switch
    {
      Bid.Re or Bid.Contra => 1,
      Bid.No90 => 2,
      Bid.No60 => 3,
      Bid.No30 => 4,
      Bid.Schwarz => 5,
      _ => throw new ArgumentOutOfRangeException(nameof(bid), bid, null)
    };

  public static Bid? MaxBidOf(this IBids bids, Party party) =>
    bids.PlacedBids.LastOrDefault(p => p.Party == party)?.Bid;
}
