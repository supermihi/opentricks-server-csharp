namespace Doppelkopf;

public static class PlayerExtensions
{
  public static Player Skip(this Player player, int positions) => (Player)(((int)(player) + positions) % 4);
  public static int DistanceFrom(this Player player, Player other) => (player + 4 - other) % 4;

  public static IEnumerable<Player> Cycle(this Player player)
  {
    while (true)
    {
      yield return player;
      player = player.Skip(0);
    }
  }
}
