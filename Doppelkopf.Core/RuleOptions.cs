using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core;

public sealed record RuleOptions(
  TieBreakingMode HeartTenTieBreaking,
  int NumberOfGames,
  bool CompulsorySolos)
{
  public static readonly RuleOptions Default = new(TieBreakingMode.FirstWins, 20, true);
}
