namespace Doppelkopf.API.Views;

public sealed record ContractView(string? HoldId, Player? Declarer, bool IsCompulsorySolo);
