namespace Doppelkopf.Server.Model;

public record struct TableId(string Id)
{
  public static TableId New() => new(Guid.NewGuid().ToString());

  public static implicit operator string(TableId id) => id.Id;
}
