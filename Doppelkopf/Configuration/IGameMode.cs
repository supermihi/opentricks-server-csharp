using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public interface IGameMode
{
  GameModeKind Kind { get; }
  ITrickRules TrickRules { get; }
}
