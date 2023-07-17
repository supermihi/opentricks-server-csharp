using Doppelkopf.Errors;

namespace Doppelkopf.Core;

public static class ErrorCodes
{
  public static readonly ErrorCode NotYourTurn = new("not_your_turn", "it's not your turn");
  public static readonly ErrorCode CardNotOwned = new("card_not_owned", "you do not own this card");
  public static readonly ErrorCode CardNotAllowed = new("card_not_allowed", "you must follow suit");
  public static readonly ErrorCode InvalidPhase = new("invalid_phase", "the specified action is not valid in the current phase of the game");
  public static readonly ErrorCode ContractNotAvailable = new(
    "contract_not_available",
    "the specified contract is not available in this game");
  public static readonly ErrorCode ContractNotAllowed = new("contract_not_allowed", "the selected contract is not allowed with your cards");
  public static readonly ErrorCode PartyNotDefined = new(
    "parties_not_defined",
    "the specified action is not valid because your party is not yet defined");
  public static readonly ErrorCode WrongParty = new("wrong_party", "the specified action is not valid for your party");
  public static readonly ErrorCode RedundantBid = new(
    "redundant_bid",
    "the bid you specified has already been declared");
  public static readonly ErrorCode BidToLate = new(
    "bid_too_late",
    "you have played too many cards already to still declare the specified bid");

  public static readonly ErrorCode InvalidSeat = new("invalid_seat", "the specified seat is invalid");
  public static readonly ErrorCode SeatPaused = new("seat_paused", "your seat is currently paused, you cannot play");
}
