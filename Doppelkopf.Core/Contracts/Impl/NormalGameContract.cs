using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Contracts.Impl;

/// <summary>
/// Normal game contract (no wedding).
/// </summary>
internal class NormalGameContract(TieBreakingMode heartsTenTieBreaking, IReadOnlyDictionary<Player, bool> hasClubQueen)
  : IContract
{
  public ICardTraitsProvider Traits { get; } = CardTraitsProvider.NormalGame(heartsTenTieBreaking);

  public IPartyProvider Parties { get; } =
    new StaticPartyProvider(ByPlayer.Init(p => hasClubQueen[p] ? Party.Re : Party.Contra));

  public void OnTrickFinished(CompleteTrick trick)
  {
  }
}
