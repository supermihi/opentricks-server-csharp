using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Scoring.Impl;

public class CharlieMiller : IExtraPointRule
{
  private static readonly Card Charlie = new(Suit.Clubs, Rank.Jack);

  public IEnumerable<ExtraPoint> Evaluate(CompleteTrick trick, IPartyProvider parties)
  {
    if (trick.Remaining == 0 && parties.GetParty(trick.Winner) is { } party && trick.WinningCard == Charlie)
    {
      return new[] { new ExtraPoint(ExtraPointKind.CharlieMiller, party, trick.Index) };
    }
    return Enumerable.Empty<ExtraPoint>();
  }
}
