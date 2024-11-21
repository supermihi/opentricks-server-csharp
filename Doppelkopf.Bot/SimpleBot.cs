using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Games;
using Doppelkopf.Errors;

namespace Doppelkopf.Bot;

public class SimpleBot(Player me) : IBot
{
  public async Task OnGameStateChanged(GameView state, IDoppelkopfApi player)
  {
    if (state.Turn != me)
    {
      return;
    }
    switch (state.Phase)
    {
      case GamePhase.Finished:
        return;
      case GamePhase.Auction:
        await player.Play(PlayerAction.Declare.Fine());
        break;
      case GamePhase.TrickTaking:
        await PlayFirstValidCard(state, player);
        break;
    }
  }
  private static async Task PlayFirstValidCard(GameView state, IDoppelkopfApi player)
  {
    foreach (var card in state.OwnCards)
    {
      var error = await player.Play(card);
      if (error is null || error.Value.Code != ErrorCodes.CardNotAllowed.Code)
      {
        return;
      }
    }
  }
}
