using System.Collections;
using System.Collections.Immutable;

namespace Doppelkopf.Configuration;

public sealed class GameModeCollection : IEnumerable<IGameMode>
{
  public IGameMode NormalGame { get; }
  public IGameMode Marriage { get; }

  public GameModeCollection(IReadOnlyCollection<IGameMode> modes)
  {
    NormalGame = modes.Single(m => m.Kind == GameModeKind.NormalGame);
    Marriage = modes.Single(m => m.Kind == GameModeKind.Marriage);
    Solos = modes.Where(m => m.Kind == GameModeKind.Solo).ToImmutableList();
    SpecialGames = modes.Where(m => m.Kind == GameModeKind.Special).ToImmutableList();
  }

  public GameModeCollection(params IGameMode[] modes)
    : this((IReadOnlyCollection<IGameMode>)modes) { }

  public IImmutableList<IGameMode> SpecialGames { get; set; }

  public IImmutableList<IGameMode> Solos { get; }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public IEnumerator<IGameMode> GetEnumerator()
  {
    return new[] { NormalGame, Marriage }.Concat(Solos).Concat(SpecialGames).GetEnumerator();
  }
}
