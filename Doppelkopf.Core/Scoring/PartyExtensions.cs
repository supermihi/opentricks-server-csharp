using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring;

public static class PartyExtensions
{
  public static Party Other(this Party p) => p == Party.Re ? Party.Contra : Party.Re;

  public static ByPlayer<Party> GetAll(this IPartyProvider parties) => ByPlayer.Init(p => parties.Get(p)!.Value);
}
