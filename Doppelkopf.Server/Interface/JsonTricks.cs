using Doppelkopf.Games;

namespace Doppelkopf.Server.Interface;

public sealed record JsonTricks(JsonByPlayer<string?> CurrentTrick, JsonByPlayer<int> TricksWon)
{
  public static JsonTricks FromTrickTaking(TrickTaking trickTaking)
  {
    var currentTrick = JsonByPlayer.FromInTurns(
      trickTaking.CurrentTrick!.Data,
      (_, card) => card.Id,
      null);
    var tricksWon = JsonByPlayer.Create(p => trickTaking.CompleteTricks.Count(trick => trick.Winner == p));
    return new(currentTrick, tricksWon);
  }
}
