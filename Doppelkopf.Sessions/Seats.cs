using Doppelkopf.Core;
using Doppelkopf.Core.Utils;
using Microsoft.VisualBasic;

namespace Doppelkopf.Sessions;

public static class Seats
{
  public static ByPlayer<Seat> GetActiveSeats(int numberOfSeats,
    int gamesPlayed,
    int dealingsRepeated)
  {
    var dealerIndex = gamesPlayed - dealingsRepeated;
    var numSkipped = numberOfSeats - Rules.NumPlayers;
    var player1 = new Seat((dealerIndex + numSkipped) % Rules.NumPlayers);
    var player2 = player1.Next(numberOfSeats);
    var player3 = player2.Next(numberOfSeats);
    var player4 = player3.Next(numberOfSeats);
    return new ByPlayer<Seat>(player1, player2, player3, player4);
  }
}
