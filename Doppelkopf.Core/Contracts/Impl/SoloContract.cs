using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

internal record SoloContract : IContract
{
  public SoloContract(ICardTraitsProvider traits, Player soloist)
  {
    Traits = traits;
    Parties = StaticPartyProvider.Solo(soloist);
  }

  public IPartyProvider Parties { get; }
  public ICardTraitsProvider Traits { get; }

  public void OnTrickFinished(CompleteTrick trick)
  {
  }
}
