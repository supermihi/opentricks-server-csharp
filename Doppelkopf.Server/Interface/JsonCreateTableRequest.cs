using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.Interface;

public sealed record JsonCreateTableRequest(string Name, RuleSet RuleSet, int MaxSeats = Constants.MaxSeats,
  IReadOnlyList<UserId>? InvitedUsers = null);
