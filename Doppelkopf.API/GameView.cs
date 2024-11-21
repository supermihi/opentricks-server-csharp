using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Games;

namespace Doppelkopf.API;

public sealed record GameView(
  Player Player,
  IReadOnlyList<Card> OwnCards,
  GamePhase Phase,
  Player? Turn,
  AuctionView? Auction,
  TrickTakingView? TrickTaking);
