using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Games;

public sealed record PlayCardResult(CompleteTrick? CompletedTrick, GameEvaluation? CompletedGame);
