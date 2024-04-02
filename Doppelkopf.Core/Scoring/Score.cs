namespace Doppelkopf.Core.Scoring;

public sealed record Score(string Id, Party Party, int Amount = 1, Player? Player = null, int? Trick = null);
