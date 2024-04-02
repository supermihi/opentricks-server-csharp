using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public class CaughtTheFox : IExtraScoreRule
{
  private static readonly Card Fox = new(Suit.Diamonds, Rank.Ace);

  public IEnumerable<Score> Evaluate(IReadOnlyList<CompleteTrick> tricks, ByPlayer<Party> parties,
    Party? winnerOfGame)
  {
    if (parties.Soloist() is not null)
    {
      yield break;
    }
    foreach (var trick in tricks)
    {
      var winnerParty = parties[trick.Winner];
      var caughtFoxes = trick.Cards.Items.Count(t => t.value == Fox && parties[t.player] != winnerParty);
      for (var i = 0; i < caughtFoxes; ++i)
      {
        yield return new Score(ScoreIds.CaughtTheFox, winnerParty, 1, trick.Winner, trick.Index);
      }
    }
  }
}
