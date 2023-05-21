namespace Doppelkopf.Errors;

public static class Err
{
  public abstract record ErrorCollection(string Component, string Action)
  {
    protected InputException Create(string code, string message) =>
      new(Component, Action, code, message);

    public InputException InvalidPhase =>
      Create(
        "invalid_action",
        "the action you requested to perform is invalid in the current state of the game"
      );

    public InputException NotYourTurn => Create("not_your_turn", "it is not your turn");
    public InputException SeatPaused =>
      Create("seat_paused", "you do not participate in the current game");
  }

  public static class Auction
  {
    public record ReserveCollection() : ErrorCollection(Components.Auction, "state_reservation");

    public static readonly ReserveCollection Reserve = new();

    public record DeclareCollection() : ErrorCollection(Components.Auction, "declare")
    {
      public InputException NotReserved =>
        Create("not_reserved", "cannot declare without having reserved");

      public InputException NormalGame => Create("normal_game", "cannot declare a normal game");

      public InputException InvalidGameMode =>
        Create("invalid_mode", "the specified game mode is not configured for this game");
    }

    public static readonly DeclareCollection Declare = new();
  }

  public static class TrickTaking
  {
    public static readonly PlayCardCollection PlayCard = new();

    public record PlayCardCollection() : ErrorCollection(Components.TrickTaking, "play_card")
    {
      public InputException Forbidden =>
        Create("forbidden_card", "the card is forbidden, choose another one");

      public InputException DoNotHaveCard =>
        Create("does_not_have_card", "you do not have the card you are trying to play");
    }
  }

  public static class Table
  {
    public record StartGameCollection() : ErrorCollection(Components.Table, "start_game")
    {
      public InputException IsComplete => Create("table_complete", "the table is already complete");
    }

    public static InputException NotInitialized =
      new(Components.Table, "generic", "not_initialized", "the table is not initialized");

    public static StartGameCollection StartGame = new();
  }
}
