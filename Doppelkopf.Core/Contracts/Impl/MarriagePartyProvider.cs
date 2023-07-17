using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

public class MarriagePartyProvider : IPartyProvider
{
  private readonly Player _declarer;
  private Player? _spouse;

  public MarriagePartyProvider(Player declarer)
  {
    _declarer = declarer;
  }

  public const int MaxDefiningTrick = 2;

  public void OnTrickFinished(CompleteTrick trick)
  {
    if (DefiningTrick.HasValue)
    {
      return;
    }
    if (trick.Winner != _declarer || trick.Index == MaxDefiningTrick)
    {
      _spouse = trick.Winner;
      DefiningTrick = trick.Index;
    }
  }

  public Party? GetParty(Player player)
  {
    if (player == _declarer)
    {
      return Party.Re;
    }
    if (DefiningTrick is null)
    {
      return null;
    }
    return player == _spouse ? Party.Re : Party.Contra;
  }

  public int? DefiningTrick { get; private set; }
}
