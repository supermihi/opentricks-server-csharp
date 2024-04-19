using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts;

public interface IContract
{
  void OnTrickFinished(CompleteTrick trick);
  ICardTraitsProvider Traits { get; }
  IPartyProvider Parties { get; }
}
