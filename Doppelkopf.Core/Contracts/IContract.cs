using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts;

public interface IContract : ICardTraitsProvider, IPartyProvider
{
  void OnTrickFinished(CompleteTrick trick);
}
