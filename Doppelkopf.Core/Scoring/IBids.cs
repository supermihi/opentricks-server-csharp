namespace Doppelkopf.Core.Scoring;

public interface IBids
{
  void PlaceBid(Player player, Bid bid);
  IReadOnlyList<PlacedBid> PlacedBids { get; }
}

public sealed record PlacedBid(Party Party, Bid Bid);
