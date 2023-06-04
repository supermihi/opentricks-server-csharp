using System.Collections.Immutable;
using Doppelkopf.Games;
using Doppelkopf.Utils;

namespace Doppelkopf.Sessions;

public sealed record FinishedGames(IImmutableList<CompleteGame> Games)
{
  public static readonly FinishedGames Empty = new(ImmutableArray<CompleteGame>.Empty);

  public FinishedGames AddGame(Game game, ByPlayer<Seat> seats)
  {
    if (game.TrickTaking?.IsFinished is not false)
    {
      throw new ArgumentException("can only add finished games");
    }
    var finishedGame = new CompleteGame(game, seats, new ByPlayer<int>(0, 0, 0, 0));
    return new(Games.Add(finishedGame));
  }

  public bool HasPlayedCompulsorySolo(Seat seat)
  {
    return Games.Any(g => g.CompulsorySoloist == seat);
  }
}
