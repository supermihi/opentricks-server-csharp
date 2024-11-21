namespace Doppelkopf.Errors;

public class InvalidMoveException(string code, string message) : Exception(message)
{
  public string Code { get; } = code;

  public InvalidMoveException(ErrorCode error) : this(error.Code, error.Message)
  {
  }

  public ErrorCode ErrorCode => new(Code, Message);
}
