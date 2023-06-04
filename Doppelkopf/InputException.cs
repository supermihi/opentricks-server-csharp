namespace Doppelkopf;

public class InputException : Exception, IEquatable<InputException>
{
  public string Code { get; }

  public InputException(string code, string message)
      : base(message)
  {
    Code = code;
  }

  public bool Equals(InputException? other)
  {
    return other != null && other.Code == Code;
  }
}
