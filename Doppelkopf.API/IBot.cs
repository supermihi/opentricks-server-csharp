namespace Doppelkopf.API;

public interface IBot
{
  Task OnGameStateChanged(GameView state, IDoppelkopfApi player);
}
