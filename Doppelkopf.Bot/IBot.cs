using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Errors;

namespace Doppelkopf.Bot;

public interface IBot
{
  Task OnGameStateChanged(GameView state, IGameForPlayer game);
}

public class SimpleBot(Player me) : IBot
{
  public async Task OnGameStateChanged(GameView state, IGameForPlayer game)
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
          await game.PlayCard(card);
          return;
        }
        catch (InvalidMoveException e) when (e.Code == ErrorCodes.CardNotAllowed.Code)
        {
          // continue
        }
      }

      throw new InvalidOperationException("should not be here");
    }

    await game.DeclareHold(null);
  }
}
