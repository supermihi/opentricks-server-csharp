namespace Doppelkopf.Core.Scoring.Impl;

internal class StaticPartyProvider(IReadOnlyDictionary<Player, Party> parties) : IPartyProvider
{
  public Party? Get(Player player) => parties[player];

  public int? DefiningTrick => 0;
}
