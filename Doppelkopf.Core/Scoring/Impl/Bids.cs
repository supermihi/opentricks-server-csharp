using Doppelkopf.API.Errors;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Tricks.Impl;

namespace Doppelkopf.Core.Scoring.Impl;

public class Bids(IPartyProvider parties, ITrickTakingProgress trickTaking) : IBids
{
  public void PlaceBid(Player player, Bid bid)
  {
    var definingTrick = EnsureDefiningTrickSet();
    var party = EnsurePartyIsDefined(player);
    EnsureBidCompatibleWithParty(bid, party);
    EnsureBidNotRedundant(bid, party);
    var maxPlayedCards = bid.MaximumPlayedCards() + definingTrick;
    if (trickTaking.NumCardsPlayed(player) > maxPlayedCards)
    {
      throw ErrorCodes.BidToLate.ToException();
    }
    _placedBids.Add(new PlacedBid(party, bid));
  }

  public IReadOnlyList<PlacedBid> PlacedBids => _placedBids;

  private void EnsureBidNotRedundant(Bid bid, Party party)
  {
    if (this.MaxBidOf(party)?.IsOrImplies(bid) ?? false)
    {
      throw ErrorCodes.RedundantBid.ToException();
    }
  }

  private int EnsureDefiningTrickSet()
  {
    if (parties.DefiningTrick is { } definingTrick)
    {
      return definingTrick;
    }
    throw ErrorCodes.PartyNotDefined.ToException();
  }

  private Party EnsurePartyIsDefined(Player player)
  {
    if (parties.Get(player) is { } party)
    {
      return party;
    }
    throw ErrorCodes.PartyNotDefined.ToException();
  }

  private static void EnsureBidCompatibleWithParty(Bid bid, Party party)
  {
    if ((bid == Bid.Re && party == Party.Contra) || (bid == Bid.Contra && party == Party.Re))
    {
      throw ErrorCodes.WrongParty.ToException();
    }
  }

  private readonly List<PlacedBid> _placedBids = [];
}
