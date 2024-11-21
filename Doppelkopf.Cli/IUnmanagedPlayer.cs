using Doppelkopf.API;
using Doppelkopf.API.Views;

namespace Doppelkopf.Cli;

public interface IUnmanagedPlayer
{
  Task OnStateChanged(GameView view);
  void StartGame(IDoppelkopfApi player);
}
