using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Games;

public interface IGameFactory
{
  IGame CreateGame(IByPlayer<bool> needsCompulsorySolo);
}
