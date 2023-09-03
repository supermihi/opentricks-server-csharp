namespace Doppelkopf.Core.Scoring.Impl;

internal static class BidExtensions
{
  internal static int ExtraScore(this Bid bid) =>
      bid switch
      {
        Bid.Re or Bid.Contra => 2,
        Bid.NoNinety => 3,
        Bid.NoSixty => 4,
        Bid.NoThirty => 5,
        Bid.Schwarz => 6,
        _ => throw new ArgumentOutOfRangeException(nameof(bid), bid, null)
      };
}
