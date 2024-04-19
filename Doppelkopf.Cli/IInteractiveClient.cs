using Doppelkopf.API;

namespace Doppelkopf.Cli;

public interface IInteractiveClient
{
  Task OnStateChanged(GameView view);
  void StartGame(IPlayerClient player);
}
