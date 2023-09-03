namespace Doppelkopf.Sessions;

public sealed record SessionConfiguration(int NumberOfGames, bool CompulsorySolos, int NumberOfSeats)
{
  public static SessionConfiguration DDKV => new(NumberOfGames: 24, CompulsorySolos: true, NumberOfSeats: 4);
}
