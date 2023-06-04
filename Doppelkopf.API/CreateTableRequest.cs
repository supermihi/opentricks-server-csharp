namespace Doppelkopf.API;

public sealed record CreateTableRequest(string Name, RuleSet RuleSet, int MaxSeats = 4,
  IReadOnlyList<string>? InvitedUsers = null);
