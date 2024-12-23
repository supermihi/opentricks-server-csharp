using Doppelkopf.API;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Sessions;

public interface ISession
{
  IGame CurrentGame { get; set; }
  ByPlayer<Seat> Players { get; set; }
  Player ActivePlayer(Seat seat);
  public void Play(Seat seat, PlayerAction action);
}
