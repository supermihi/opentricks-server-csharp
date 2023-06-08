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

  public static Rules FromCreateTableRequest(CreateTableRequest request)
  {
    if (request.MaxSeats is < Constants.NumberOfPlayers or > Constants.MaxSeats)
    {
      throw new ArgumentException("invalid number of seats");
    }
    return new Rules(request.RuleSet, request.MaxSeats);
  }
}
