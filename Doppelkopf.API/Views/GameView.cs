namespace Doppelkopf.API.Views;

public sealed record GameView(
  Player Player,
  IReadOnlyList<Card> OwnCards,
  GamePhase Phase,
  Player? Turn,
  AuctionView? Auction,
  TrickTakingView? TrickTaking);
