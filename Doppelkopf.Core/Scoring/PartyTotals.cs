using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring;

public sealed record PartyTotals(int Points, bool Schwarz, Bid? MaxBid)
{
  public static PartyTotals Compute(Party party, IReadOnlyCollection<CompleteTrick> tricks, ByPlayer<Party> parties,
    Bid? maxBid)
  {
    var points = tricks.Where(t => parties[t.Winner] == party).Sum(t => t.Points());
    var schwarz = tricks.All(t => parties[t.Winner] != party);
    return new PartyTotals(points, schwarz, maxBid);
  }

  public static ByParty<PartyTotals> ComputeBoth(IReadOnlyCollection<CompleteTrick> tricks, ByParty<Bid?> maxBids,
    ByPlayer<Party> parties) =>
    ByParty.Init(p => Compute(p, tricks, parties, maxBids[p]));
}
