using Doppelkopf.Core;

namespace Doppelkopf.API;

public sealed record ContractView(string? HoldId, Player? Declarer, bool IsCompulsorySolo);
