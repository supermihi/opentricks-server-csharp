namespace Doppelkopf.Configuration;

public sealed record RuleSet(
  bool WithoutNines,
  bool ThrowOnFiveNines,
  int NumberOfGames,
  bool CompulsorySolos
)
{
  public static RuleSet DDKV = new(false, false, 20, true);
}
