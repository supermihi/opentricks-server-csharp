using System.Text.Json.Serialization;

namespace Doppelkopf.Server.Interface;

public sealed record JsonCreateTableRequest([property: JsonPropertyName("name"), JsonRequired]
  string Name);
