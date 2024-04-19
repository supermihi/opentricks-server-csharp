using Doppelkopf.Core;

namespace Doppelkopf.API;

public record ContractView(string? HoldId, Player? Declarer, bool IsCompulsorySolo);
