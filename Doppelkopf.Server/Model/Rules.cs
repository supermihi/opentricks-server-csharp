using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Model;

public sealed record Rules(RuleSet RuleSet, int MaxSeats)
{
  public GameConfiguration GameConfiguration()
  {
    return RuleSet switch
    {
      RuleSet.DDKV => new GameConfiguration(TieBreakingMode.FirstWins, true),
      RuleSet.Minikopf => throw new NotImplementedException(),
      _ => throw new ArgumentOutOfRangeException()
    };
  }

  public SessionConfiguration SessionConfiguration() => new(24, true);

  public static Rules FromCreateTableRequest(CreateTableRequest request)
  {
    if (request.MaxSeats is < Core.Rules.NumPlayers or > 5)
    {
      throw new ArgumentException("invalid number of seats");
    }
    return new Rules(request.RuleSet, request.MaxSeats);
  }
}
