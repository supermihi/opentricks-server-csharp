using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Tricks.Impl;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Scoring.Impl;

public class ScoreController : IBids
{
  private readonly IPartyProvider _parties;
  private readonly ITrickTakingProgress _trickTaking;

  public ScoreController(IPartyProvider parties, ITrickTakingProgress trickTaking)
  {
    _parties = parties;
    _trickTaking = trickTaking;
  }

  public void PlaceBid(Player player, Bid bid)
  {
    if (_parties.GetParty(player) is not { } party || _parties.DefiningTrick is not {} definingTrick)
    {
      throw ErrorCodes.PartyNotDefined.ToException();
    }
    if ((bid == Bid.Re && party == Party.Contra) || (bid == Bid.Contra && party == Party.Re))
    {
      throw ErrorCodes.WrongParty.ToException();
    }
    if (_placedBids.Any(b => b.Party == party && b.Bid.IsOrImplies(bid)))
    {
      throw ErrorCodes.RedundantBid.ToException();
    }
    var maxPlayedCards = bid.MaximumPlayedCards() + definingTrick;
    if (_trickTaking.NumCardsPlayed(player) > maxPlayedCards)
    {
      throw ErrorCodes.BidToLate.ToException();
    }
    _placedBids.Add(new PlacedBid(player, party, bid, _trickTaking.CurrentTrick!.Index));
  }

  private sealed record PlacedBid(Player Player, Party Party, Bid Bid, int Trick);

  private readonly List<PlacedBid> _placedBids = new();
}
