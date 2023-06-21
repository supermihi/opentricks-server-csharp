using Doppelkopf.API;

namespace Doppelkopf.Server.Model;

public sealed record TableMeta(TableId Id, DateTime CreatedUtc, string Name, UserId Owner, Rules Rules)
{
  public static TableMeta Create(string name, UserId creator, Rules rules)
  {
    var tableId = TableId.New();
    return new TableMeta(tableId, DateTime.UtcNow, name, creator, rules);
  }
}
