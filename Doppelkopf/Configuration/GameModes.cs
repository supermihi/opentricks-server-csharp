using System.Collections.Immutable;

namespace Doppelkopf.Configuration;

public sealed class GameModes : IGameModes
{
  public IGameMode NormalGame { get; }
  public IGameMode Marriage { get; }

  public IReadOnlyCollection<IGameMode> SpecialGames { get; }

  public IReadOnlyCollection<IGameMode> Solos { get; }

  public GameModes(IReadOnlyCollection<IGameMode> modes)
  {
    NormalGame = modes.Single(m => m.Kind == GameModeKind.NormalGame);
    Marriage = modes.Single(m => m.Kind == GameModeKind.Marriage);
    Solos = modes.Where(m => m.Kind == GameModeKind.Solo).ToImmutableList();
    SpecialGames = modes.Where(m => m.Kind == GameModeKind.Special).ToImmutableList();
  }

  public GameModes(params IGameMode[] modes)
    : this((IReadOnlyCollection<IGameMode>)modes) { }
}
