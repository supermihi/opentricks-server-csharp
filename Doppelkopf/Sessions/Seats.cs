using Doppelkopf.Utils;

namespace Doppelkopf.Sessions;

public static class Seats
{
  public static ByPlayer<Seat> GetActiveSeats(int numberOfSeats,
    int gamesPlayed,
    int dealingsRepeated)
  {
    var dealerIndex = gamesPlayed - dealingsRepeated;
    var numSkipped = numberOfSeats - Constants.NumberOfPlayers;
    var player1 = new Seat((dealerIndex + numSkipped) % Constants.NumberOfPlayers);
    var player2 = player1.Next(numberOfSeats);
    var player3 = player2.Next(numberOfSeats);
    var player4 = player3.Next(numberOfSeats);
    return new ByPlayer<Seat>(player1, player2, player3, player4);
  }

  public static ByPlayer<Seat> NextGameActiveSeats(this FinishedGames history, int numberOfSeats) =>
      GetActiveSeats(
        numberOfSeats,
        history.Games.Count,
        history.Games.Count(g => g.CompulsorySoloist.HasValue)
      );
}
