using System.Text.Json.Serialization;

namespace Doppelkopf.API;

public sealed record TableState([property: JsonRequired]
  string Id,
  [property: JsonRequired]
  string Name,
  [property: JsonRequired]
  string Owner,
  IReadOnlyList<string> Players,
  int Version,
  bool Started,
  RuleSet RuleSet,
  int MaxSeats);
