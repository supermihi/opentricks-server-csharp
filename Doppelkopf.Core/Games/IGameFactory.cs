using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Games;

public interface IGameFactory
{
  IGame CreateGame(ByPlayer<bool> needsCompulsorySolo);
}
