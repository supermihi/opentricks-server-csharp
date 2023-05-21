using System.Collections.Immutable;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.Games;
using Doppelkopf.Scoring;

namespace Doppelkopf.Sessions;

public sealed record GameHistory(
  IImmutableList<FinishedGame> Games,
  IImmutableDictionary<Seat, bool> NeedsCompulsorySolo
)
{
  public static GameHistory Init(int numberOfSeats, bool compulsorySolos)
  {
    return new(
      ImmutableArray<FinishedGame>.Empty,
      Enumerable
        .Range(0, numberOfSeats)
        .ToImmutableDictionary(s => new Seat(s), s => compulsorySolos)
    );
  }

  private GameHistory AddGame(FinishedGame game)
  {
    return new(
      Games.Add(game),
      game.IsCompulsorySolo
        ? NeedsCompulsorySolo.SetItem(game.Players[game.Contract.Soloist!.Value], false)
        : NeedsCompulsorySolo
    );
  }

  public GameHistory AddGame(Game game, ByPlayer<Seat> seats)
  {
    if (game.TrickTaking?.IsFinished is not false)
    {
      throw new IllegalStateException("cannot create completed game from unfinished game");
    }
    var isSolo = game.Contract!.Mode.Kind == GameModeKind.Solo;
    var isCompulsory = isSolo && NeedsCompulsorySolo[seats[game.Contract!.Soloist!.Value]];

    var finishedGame = new FinishedGame(
      game.TrickTaking.Contract,
      isCompulsory,
      seats,
      new ByPlayer<int>(0, 0, 0, 0) // TODO
    );
    return AddGame(finishedGame);
  }
}
