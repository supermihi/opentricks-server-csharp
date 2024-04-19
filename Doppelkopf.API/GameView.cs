using Doppelkopf.Core;
using Doppelkopf.Core.Cards;

namespace Doppelkopf.API;

public record GameView(
  Player Player,
  IReadOnlyList<Card> OwnCards,
  Player? Turn,
  AuctionView? Auction,
  TrickTakingView? TrickTaking);
