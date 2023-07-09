namespace Doppelkopf.Errors;

public class InvalidMoveException : Exception
{
  public string Code { get; }

  public InvalidMoveException(string code, string message) : base(message)
  {
    Code = code;
  }

  public InvalidMoveException(ErrorCode error) : this(error.Code, error.Message)
  { }
}
