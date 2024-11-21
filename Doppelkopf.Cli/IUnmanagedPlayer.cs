using Doppelkopf.API;

namespace Doppelkopf.Cli;

public interface IUnmanagedPlayer
{
  Task OnStateChanged(GameView view);
  void StartGame(IPlayerClient player);
}
