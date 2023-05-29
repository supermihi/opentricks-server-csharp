using Doppelkopf.Cards;
using Doppelkopf.Games;
using Doppelkopf.Tricks;

namespace Doppelkopf.Sessions;

public static class SessionExtensions
{
  public static (Session result, CompleteTrick? completeTrick, CompleteGame? completeGame) PlayCardAndProceed(
    this Session session,
    Seat seat, Card card)
  {
    var (result, finishTrick) = session.PlayCard(seat, card);
    if (!finishTrick)
    {
      return (result, null, null);
    }
    (result, var finishGame) = session.FinishTrick();
    var finishedTrick = result.Game.TrickTaking!.CompleteTricks.Last();
    if (finishGame)
    {
      result = result.FinishGame();
      var completedGame = result.CompleteGames.Games.Last();
      return (result, finishedTrick, completedGame);
    }
    return (result, finishedTrick, null);
  }
}
