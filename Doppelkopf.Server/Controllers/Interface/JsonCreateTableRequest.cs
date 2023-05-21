using System.Text.Json.Serialization;

namespace Doppelkopf.Server.Controllers.Interface;

public sealed record JsonCreateTableRequest(
  [property: JsonPropertyName("name"), JsonRequired] string Name
);
