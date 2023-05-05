using System.Collections.Immutable;

namespace Doppelkopf.Configuration;

public sealed class GameModeCollection
{
  public IGameMode DefaultNormalGame { get; }
  public IGameMode Marriage { get; }

  public GameModeCollection(
    IGameMode defaultNormalGame,
    IGameMode marriage,
    IEnumerable<IGameMode> solos,
    IEnumerable<IGameMode> additionalNormalGames
  )
  {
    if (defaultNormalGame.Kind != GameModeKind.NormalGame)
    {
      throw new ArgumentException($"{nameof(defaultNormalGame)} must be a normal game");
    }
    DefaultNormalGame = defaultNormalGame;
    if (marriage.Kind != GameModeKind.Marriage)
    {
      throw new ArgumentException($"{nameof(marriage)} must be a marriage game");
    }
    Marriage = marriage;
    Solos = solos.ToImmutableList();
    if (Solos.Any(solo => solo.Kind != GameModeKind.Solo))
    {
      throw new ArgumentException($"{nameof(solos)} must contain solos only");
    }
    AdditionalNormalGames = additionalNormalGames.ToImmutableList();
    if (AdditionalNormalGames.Any(game => game.Kind != GameModeKind.NormalGame))
    {
      throw new ArgumentException(
        $"{nameof(additionalNormalGames)} must contain normal games only"
      );
    }
  }

  public IImmutableList<IGameMode> AdditionalNormalGames { get; set; }

  public IImmutableList<IGameMode> Solos { get; }
}
