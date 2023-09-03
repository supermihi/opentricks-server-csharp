namespace Doppelkopf.Core.Scoring;

public static class PartyExtensions
{
  public static Party Other(this Party p) => p == Party.Re ? Party.Contra : Party.Re;
}
