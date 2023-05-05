namespace Doppelkopf.Errors;

public class InputException : Exception, IEquatable<InputException>
{
  private readonly string _code;
  public Component Component { get; }

  public InputException(Component component, string action, string code, string message)
    : base(message)
  {
    _code = code;
    Component = component;
  }

  public bool Equals(InputException? other)
  {
    return other != null && other.Component == Component && other._code == _code;
  }

  public static class Table
  {
    private const Component Component = Errors.Component.Table;

    public static class StartGame
    {
      private const string Action = "start_game";

      public static InputException TableComplete =>
        new(Component, Action, "table_complete", "the table is already complete");

      public static InputException GameInProgress =>
        new(Component, Action, "game_in_progress", "there is a game in progress");
    }
  }

  public static class Game
  {
    private const Component Component = Errors.Component.Game;

    public static class PlayCard
    {
      private const string Action = "play_card";

      public static InputException GameFinished =>
        new(Component, Action, "game_finished", "the game is finished, cannot play card");

      public static InputException NotYourTurn =>
        new(Component, Action, "not_your_turn", "it is not your turn");

      public static InputException ForbiddenCard =>
        new(Component, Action, "forbidden_card", "the card is forbidden, choose another one");

      public static InputException DoNotHaveCard =>
        new(
          Component,
          Action,
          "does_not_have_card",
          "you do not have the card you are trying to play"
        );
    }
  }
}

public enum Component
{
  Table,
  Game
}
