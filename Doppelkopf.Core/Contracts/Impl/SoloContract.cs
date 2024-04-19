using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Contracts.Impl;

internal record SoloContract : IContract
{
  public SoloContract(ICardTraitsProvider traits, Player soloist)
  {
    Traits = traits;
    Parties = new StaticPartyProvider(ByPlayer.Init(p => p == soloist ? Party.Re : Party.Contra));
  }

  public IPartyProvider Parties { get; }
  public ICardTraitsProvider Traits { get; }

  public void OnTrickFinished(CompleteTrick trick)
  {
  }
}
