namespace Doppelkopf.Tricks;

public sealed record TrickContext(ITrickRules Rules, bool IsLastTrick);
