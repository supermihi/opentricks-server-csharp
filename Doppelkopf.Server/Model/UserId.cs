namespace Doppelkopf.Server.Model;

public record struct UserId(string Id)
{
  public static implicit operator string(UserId user) => user.Id;
}
