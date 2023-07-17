using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Scoring;

public class Doppelkopf : IExtraPointRule
{
  private const int MinScore = 40;

  public IEnumerable<ExtraPoint> Evaluate(CompleteTrick trick, IPartyProvider parties)
  {
    if (trick.Score() >= MinScore && parties.GetParty(trick.Winner) is { } party)
    {
      return new[] { new ExtraPoint(ExtraPointKind.Doppelkopf, party, trick.Index) };
    }
    return Enumerable.Empty<ExtraPoint>();
  }
}