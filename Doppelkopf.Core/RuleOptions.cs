using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core;

public sealed record RuleOptions(
  TieBreakingMode HeartTenTieBreaking,
  bool CompulsorySolos)
{
  public static readonly RuleOptions Default = new(TieBreakingMode.FirstWins, true);
}
