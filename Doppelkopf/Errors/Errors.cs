namespace Doppelkopf.Errors;

public static class Err
{
  public abstract record ErrorCollection(Component Component, string Action)
  {
    protected InputException Create(string code, string message) =>
      new(Component, Action, code, message);

    public InputException InvalidPhase =>
      Create(
        "invalid_action",
        "the action you requested to perform is invalid in the current state of the game"
      );

    public InputException NotYourTurn => Create("not_your_turn", "it is not your turn");
  }

  public static class Auction
  {
    public record ReserveCollection() : ErrorCollection(Component.Game, "state_reservation");

    public static readonly ReserveCollection Reserve = new();

    public record DeclareCollection() : ErrorCollection(Component.Game, "declare")
    {
      public InputException NotReserved =>
        Create("not_reserved", "cannot declare without having reserved");

      public InputException NormalGame => Create("normal_game", "cannot declare a normal game");

      public InputException InvalidGameMode =>
        Create("invalid_mode", "the specified game mode is not configured for this game");
    }

    public static readonly DeclareCollection Declare = new();
  }

  public static class Game
  {
    public static readonly PlayCardCollection PlayCard = new();

    public record PlayCardCollection() : ErrorCollection(Component.Game, "play_card")
    {
      public InputException Forbidden =>
        Create("forbidden_card", "the card is forbidden, choose another one");

      public InputException DoNotHaveCard =>
        Create("does_not_have_card", "you do not have the card you are trying to play");
    }
  }

  public static class Table
  {
    public record StartGameCollection() : ErrorCollection(Component.Table, "start_game")
    {
      public InputException IsComplete => Create("table_complete", "the table is already complete");
    }

    public static StartGameCollection StartGame = new();
  }
}
