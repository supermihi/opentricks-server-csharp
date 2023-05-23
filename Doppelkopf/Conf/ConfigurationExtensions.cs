using Doppelkopf.Games;

namespace Doppelkopf.Conf;

public static class ConfigurationExtensions
{
    public static GameContext InitialGameContext(this Configuration configuration)
    {
        return new GameContext(
          ByPlayer.Init(configuration.Session.CompulsorySolos),
          configuration.Contracts,
          configuration.Tricks
        );
    }
}
