namespace Doppelkopf.Core.Scoring;

public enum Bid
{
  Re,
  Contra,
  NoNinety,
  NoSixty,
  NoThirty,
  Schwarz
}

public static class BidExtensions
{
  public static bool IsOrImplies(this Bid bid, Bid other)
  {
    return other <= bid;
  }

  public static int MaximumPlayedCards(this Bid bid) =>
      bid switch
      {
        Bid.Re or Bid.Contra => 1,
        Bid.NoNinety => 2,
        Bid.NoSixty => 3,
        Bid.NoThirty => 4,
        Bid.Schwarz => 5,
        _ => throw new ArgumentOutOfRangeException(nameof(bid), bid, null)
      };
}
