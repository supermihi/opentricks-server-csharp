namespace Doppelkopf.Core.Scoring.Impl;

internal static class BidExtensions
{
  internal static int ExtraScore(this Bid bid) =>
    bid switch
    {
      Bid.Re or Bid.Contra => 2,
      Bid.No90 => 3,
      Bid.No60 => 4,
      Bid.No30 => 5,
      Bid.Schwarz => 6,
      _ => throw new ArgumentOutOfRangeException(nameof(bid), bid, null)
    };
}
