using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Scoring;

public interface IPartyProvider
{
  Party? GetParty(Player player);
  int? DefiningTrick { get; }
  void OnTrickFinished(CompleteTrick trick);
}
