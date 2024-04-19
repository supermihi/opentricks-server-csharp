using Doppelkopf.API;

namespace Doppelkopf.Bot;

public interface IBot
{
  Task OnGameStateChanged(GameView state, IPlayerClient player);
}
