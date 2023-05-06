namespace Doppelkopf.Errors;

public class InputException : Exception, IEquatable<InputException> {
  private readonly string _code;
  public Component Component { get; }
  public string Action { get; }

  public InputException(Component component, string action, string code, string message)
      : base(message) {
    _code = code;
    Component = component;
    Action = action;
  }

  public bool Equals(InputException? other) {
    return other != null && other.Component == Component && other._code == _code && other.Action == Action;
  }
}