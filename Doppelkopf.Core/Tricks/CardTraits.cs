namespace Doppelkopf.Core.Tricks;

public readonly record struct CardTraits(
  TrickSuit TrickSuit,
  int RankInTrickSuit,
  TieBreakingMode TieBreaking);
