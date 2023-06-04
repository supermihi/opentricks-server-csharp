using Doppelkopf.API;
using Doppelkopf.Utils;

namespace Doppelkopf.Server.Interface;

public static class JsonByPlayer
{
  public static ByPlayerState<TJson> FromByPlayer<TJson, T>(ByPlayer<T> byPlayer, Func<Player, T, TJson> map)
  {
    return Create(p => map(p, byPlayer[p]));
  }

  public static ByPlayerState<TJson> FromInTurns<TJson, T>(InTurns<T> inTurns, Func<Player, T, TJson> map, TJson defaultValue)
  {
    return Create(p => inTurns.TryGet(p, out var playerValue) ? map(p, playerValue) : defaultValue);
  }

  public static ByPlayerState<T> Create<T>(Func<Player, T> factory)
  {
    return new(factory(Player.Player1), factory(Player.Player2), factory(Player.Player3), factory(Player.Player4));
  }
}
