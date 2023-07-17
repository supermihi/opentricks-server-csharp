using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring;

public sealed record GameEvaluation(ByPlayer<Party> Parties, ByPlayer<int> Points,
  Party Winner, ByPlayer<int> Score);
