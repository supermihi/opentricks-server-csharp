using Doppelkopf.API;
using Doppelkopf.Core;
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

    if (state.TrickTaking is not null)
    {
      foreach (var card in state.OwnCards)
      {
        try
        {
          await player.PlayCard(card);
          return;
        }
        catch (InvalidMoveException e) when (e.Code == ErrorCodes.CardNotAllowed.Code)
        {
          // continue
        }
      }

      throw new InvalidOperationException("should not be here");
    }

    await player.DeclareHold(null);
  }
}
