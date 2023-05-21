using System.Collections;

namespace Doppelkopf.Configuration;

public interface IGameModes : IEnumerable<IGameMode>
{
  IGameMode NormalGame { get; }
  IGameMode Marriage { get; }
  IReadOnlyCollection<IGameMode> SpecialGames { get; }
  IReadOnlyCollection<IGameMode> Solos { get; }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  IEnumerator<IGameMode> IEnumerable<IGameMode>.GetEnumerator()
  {
    return new[] { NormalGame, Marriage }.Concat(Solos).Concat(SpecialGames).GetEnumerator();
  }
}