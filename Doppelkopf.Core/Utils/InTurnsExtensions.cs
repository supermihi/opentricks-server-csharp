using Doppelkopf.Errors;

namespace Doppelkopf.Core.Utils;

public static class InTurnsExtensions
{
  public static InTurns<T> AddChecked<T>(this InTurns<T> turns, Player player, T value, bool invalidPhaseIfFull = true)
  {
    CheckIsTurn(turns, player, invalidPhaseIfFull);
    return turns.Add(value);
  }

  public static void CheckIsTurn<T>(this InTurns<T> turns, Player player, bool invalidPhaseIfFull = true) {
    if (invalidPhaseIfFull && turns.IsFull) {
      ErrorCodes.InvalidPhase.Throw();
    }
    if (turns.Next != player) {
      ErrorCodes.NotYourTurn.Throw();
    }
  }
}
