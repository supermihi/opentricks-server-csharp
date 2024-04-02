using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public class CharlieMiller : IExtraScoreRule
{
  private static readonly Card Charlie = new(Suit.Clubs, Rank.Jack);

  public IEnumerable<Score> Evaluate(IReadOnlyList<CompleteTrick> tricks, ByPlayer<Party> parties,
    Party? winnerOfGame)
  {
    if (parties.Soloist() is not null)
    {
      yield break;
    }
    var lastTrick = tricks[^1];
    if (lastTrick.WinningCard == Charlie)
    {
      yield return new Score(
        ScoreIds.CharlieMiller,
        parties[lastTrick.Winner],
        1,
        lastTrick.Winner,
        lastTrick.Index);
    }
  }
}
