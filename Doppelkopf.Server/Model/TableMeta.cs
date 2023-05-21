namespace Doppelkopf.Server.Model;

public sealed record TableMeta(TableId Id, DateTime CreatedUtc, string Name, UserId Owner)
{
  public static TableMeta Init(TableId id, string name, UserId owner) =>
    new(id, DateTime.UtcNow, name, owner);
}
