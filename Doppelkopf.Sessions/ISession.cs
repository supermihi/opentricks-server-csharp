using Doppelkopf.Core;
using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Sessions;

public interface ISession
{
  IGame CurrentGame { get; set; }
  ByPlayer<Seat> Players { get; set; }
  Player ActivePlayer(Seat seat);
  public void Declare(Seat seat, Declaration declaration);
  public void PlayCard(Seat seat, Card card);
}
