using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

internal class StaticPartyProvider(IReadOnlyDictionary<Player, Party> parties) : IPartyProvider
{
  public Party? Get(Player player) => parties[player];

  public int? DefiningTrick => 0;
  internal static StaticPartyProvider Solo(Player soloist) => new(ByPlayer.Init(p => p == soloist ? Party.Re : Party.Contra));
}
