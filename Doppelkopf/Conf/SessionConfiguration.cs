namespace Doppelkopf.Conf;

public sealed record SessionConfiguration(int NumberOfGames, bool CompulsorySolos)
{
  public static SessionConfiguration DDKV => new(NumberOfGames: 24, CompulsorySolos: true);
}
