using Doppelkopf.Errors;

namespace Doppelkopf.Core;

public static class ErrorCodes
{
  public static readonly ErrorCode NotYourTurn = new("not_your_turn", "it's not your turn");
  public static readonly ErrorCode CardNotOwned = new("card_not_owned", "you do not own this card");
  public static readonly ErrorCode CardNotAllowed = new("card_not_allowed", "you must follow suit");
  public static readonly ErrorCode InvalidPhase = new("invalid_phase", "the specified action is not valid in the current phase of the game");
  public static readonly ErrorCode ContractNotAllowed = new("contract_not_allowed", "the selected contract is not allowed with your cards");
}
