using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Scoring;

public interface IExtraPointRule
{
  IEnumerable<ExtraPoint> Evaluate(CompleteTrick trick, IPartyProvider parties);
}