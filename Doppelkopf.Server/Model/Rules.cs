using Doppelkopf.API;

namespace Doppelkopf.Server.Model;

public sealed record Rules(RuleSet RuleSet, int MaxSeats)
{
  public Configuration ToConfiguration()
  {
    return RuleSet switch
    {
      RuleSet.DDKV => Configuration.DDKV,
      RuleSet.Minikopf => Configuration.Minikopf,
      _ => throw new ArgumentOutOfRangeException()
    };
  }
}
