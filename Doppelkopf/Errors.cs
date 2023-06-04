namespace Doppelkopf;

public static class Errors
{
  public static class Generic
  {
    public static InputException InvalidPhase =>
        new(
          "invalid_phase",
          "the action you tried to perform is not valid in this phase of the game");

    public static OtherPlayersTurnException OtherPlayersTurn(Player other) => new(other);
  }

  public static class Auction
  {
    public static InputException NoReservation =>
        new("no_reservation", "cannot declare without having made a reservation");

    public static InputException CannotDeclareContract =>
        new(
          "cannot_declare_contract",
          "you are not allowed to declare the selected contract");
  }

  public static class Session
  {
    public static InputException SeatPaused => new("seat_paused", "your seat is inactive in the current game");
    public static InputException IsFinished => new("session_finished", "the session has finished");
  }

  public static class TrickTaking
  {
    public static InputException TrickNotFull => new("trick_not_full", "cannot finish a trick that is not yet full");
    public static InputException InvalidCard => new("invalid_card", "you must follow suit");
    public static InputException DoNotHaveCard => new("card_not_owned", "the card you try to play is not in your hand");
  }
}

public class OtherPlayersTurnException : InputException
{
  public Player Player { get; }

  public OtherPlayersTurnException(Player player) : base("other_players_turn", $"it is {player}'s turn, not yours")
  {
    Player = player;
  }
}
