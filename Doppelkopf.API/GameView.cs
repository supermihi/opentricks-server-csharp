using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;

namespace Doppelkopf.API;

public record AuctionView(IReadOnlyList<bool> Holds);

public record ContractView(string? HoldId, Player? Declarer, bool IsCompulsorySolo);

public record BidView(Player Player, Bid Bid);

public record TrickView(Player Leader, IReadOnlyList<Card> Cards, int Index, Player? Winner);

public record TrickTakingView(
  ContractView Contract,
  TrickView? CurrentTrick,
  TrickView? PreviousTrick,
  IReadOnlyList<BidView> Bids);

public record GameView(
  Player Player,
  IReadOnlyCollection<Card> OwnCards,
  Player? Turn,
  AuctionView? Auction,
  TrickTakingView? TrickTaking);
