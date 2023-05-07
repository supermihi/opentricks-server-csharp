using Doppelkopf.Errors;
using Doppelkopf.Table;

namespace Doppelkopf.Scoring;

public record CompletedMatch(
  Contract Contract,
  bool IsCompulsorySolo,
  ByPlayer<Seat> Players,
  ByPlayer<int> Score
)
{
  public static CompletedMatch FromMatch(Match match, ByPlayer<Seat> seats, bool isCompulsory)
  {
    if (match.TrickTaking?.IsFinished ?? true)
    {
      throw new IllegalStateException("cannot create completed match from unfinished match");
    }
    return new(
      match.TrickTaking.Contract,
      isCompulsory,
      seats,
      new ByPlayer<int>(0, 0, 0, 0) // TODO
    );
  }
}
