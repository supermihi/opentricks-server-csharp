using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Games;
using Doppelkopf.Errors;

namespace Doppelkopf.Bot;

public class SimpleBot(Player me) : IBot
{
  public async Task OnGameStateChanged(GameView state, IPlayerClient player)
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
        await player.Play(PlayerAction.Declare.Healthy());
        break;
      case GamePhase.TrickTaking:
        await PlayFirstValidCard(state, player);
        break;
    }
  }
  private static async Task PlayFirstValidCard(GameView state, IPlayerClient player)
  {
    foreach (var card in state.OwnCards)
    {
      var error = await player.Play(new PlayerAction(PlayCard: new PlayCardAction(card)));
      if (error is null || error.Value.Code != ErrorCodes.CardNotAllowed.Code)
      {
        return;
      }
    }
  }
}
