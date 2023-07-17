using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public sealed record PartyProvider(ByPlayer<Party?> Parties, int? DefiningTrick) : IPartyProvider
{
  public Party? GetParty(Player p) => Parties[p];

  public void OnTrickFinished(CompleteTrick trick)
  { }

  public PartyProvider(params Player[] rePlayers) : this(
    ByPlayer.Init<Party?>(p => rePlayers.Contains(p) ? Party.Re : Party.Contra),
    0)
  { }

  public static PartyProvider Solo(Player soloist)
  {
    return new(soloist);
  }
}
