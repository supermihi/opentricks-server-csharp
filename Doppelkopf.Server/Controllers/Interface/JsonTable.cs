using System.Text.Json.Serialization;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Controllers.Interface;

public sealed record JsonTable(
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("name")] string Name,
  [property: JsonPropertyName("ownerId")] string Owner,
  [property: JsonPropertyName("players")] IReadOnlyList<string> Players
)
{
    public static JsonTable FromTableData(TableData table)
    {
        return new JsonTable(
          Id: table.Meta.Id,
          Name: table.Meta.Name,
          Owner: table.Meta.Owner,
          Players: table.Users.Users.Select(id => id.Id).ToArray()
        );
    }
}
