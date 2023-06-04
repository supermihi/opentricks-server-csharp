using Doppelkopf.Utils;

namespace Doppelkopf.Contracts;

public sealed record PartyData(ByPlayer<Party> PartyOf, int? ClarifyingTrick, Player? Declarer, Player? Soloist)
{
  public bool IsClarified => PartyOf.All(p => p != Party.NotClarified);
  public static PartyData NothingClarified => new(ByPlayer.Init(Party.NotClarified), null, null, null);
}
