using Doppelkopf.Sessions;

namespace Doppelkopf.Scoring;

public record FinishedGame(
    Contract Contract,
    bool IsCompulsorySolo,
    ByPlayer<Seat> Players,
    ByPlayer<int> Score
);
