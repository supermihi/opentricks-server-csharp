using Doppelkopf.Errors;
using Doppelkopf.Tables;

namespace Doppelkopf.Scoring;

public record CompletedMatch(
    Contract Contract,
    bool IsCompulsorySolo,
    ByPlayer<Seat> Players,
    ByPlayer<int> Score
);
