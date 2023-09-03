namespace Doppelkopf.Core.Scoring;

public interface IBids
{
    void PlaceBid(Player player, Bid bid);
    Bid? MaxBidOf(Party party);
}
