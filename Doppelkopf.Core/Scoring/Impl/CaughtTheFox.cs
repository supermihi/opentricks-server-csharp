using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Scoring;

public class CaughtTheFox : IExtraPointRule
{
  private static readonly Card Fox = new(Suit.Diamonds, Rank.Ace);

  public IEnumerable<ExtraPoint> Evaluate(CompleteTrick trick, IPartyProvider parties)
  {
    if (parties.DefiningTrick is null)
    {
      return Enumerable.Empty<ExtraPoint>();
    }
    var winnerParty = parties.GetParty(trick.Winner)!.Value;
    var caughtFoxes = trick.Cards.Items
        .Where(t => t.item == Fox && t.player != trick.Winner)
        .Count(t => parties.GetParty(t.player) != winnerParty);
    return Enumerable.Repeat(new ExtraPoint(ExtraPointKind.CatchedTheFox, winnerParty, trick.Index), caughtFoxes);
  }
}