using Doppelkopf.Cards;
using Doppelkopf.Contracts;
using Doppelkopf.Games;
using Doppelkopf.Utils;
using static Doppelkopf.Errors;

namespace Doppelkopf.Sessions;

/// <summary>
/// A session of multiple (24 with tournament rules) Doppelkopf games.
/// </summary>
/// <param name="NumberOfSeats">
/// Number of people participating in this session. Can be larger than <see cref="Constants.NumberOfPlayers"/>; in
/// that case, seats will take turns in being skipped.
/// </param>
/// <param name="Game">
/// Current game.
/// </param>
/// <param name="ActiveSeats">
/// <see cref="Seat"/> to <see cref="Player"/> mapping for the current game.
/// </param>
public sealed record Session(Configuration Configuration, int NumberOfSeats, FinishedGames CompleteGames,
  Game Game, ByPlayer<Seat> ActiveSeats)
{
  public static Session Init(Configuration configuration, int numberOfSeats, Random? random)
  {
    var cards = configuration.Cards.ShuffleCards(0);
    var activeSeats = Seats.GetActiveSeats(numberOfSeats, 0, 0);
    var history = FinishedGames.Empty;
    var context = GetGameContext(history, configuration, activeSeats);
    return new(
      configuration,
      numberOfSeats,
      history,
      Game.Init(context, cards),
      activeSeats
    );
  }

  public Session FinishGame()
  {
    if (IsFinished)
    {
      throw Errors.Session.IsFinished;
    }
    if (Game is null || !Game.IsFinished)
    {
      throw Errors.Generic.InvalidPhase;
    }
    var newHistory = CompleteGames.AddGame(Game, ActiveSeats);
    var newSeats = newHistory.NextGameActiveSeats(NumberOfSeats);
    var context = GetGameContext(newHistory, Configuration, newSeats);
    var newGame = Game.Init(context, Configuration.Cards.ShuffleCards(newHistory.Games.Count));
    return this with { Game = newGame, ActiveSeats = newSeats, CompleteGames = newHistory };
  }

  public (Session, bool finishedTrick) PlayCard(Seat seat, Card card)
  {
    var player = GetPlayer(seat);
    var (newGame, finishedTrick) = Game.PlayCard(player, card);
    var newSession = this with { Game = newGame };
    return (newSession, finishedTrick);
  }

  public (Session, bool finishedGame) FinishTrick()
  {
    var (newGame, finishedGame) = Game.FinishTrick();
    var newSession = this with { Game = newGame };
    return (newSession, finishedGame);
  }

  public Session Reserve(Seat seat, bool reserved)
  {
    var player = GetPlayer(seat);
    var next = Game.Reserve(player, reserved) ?? throw Errors.Generic.InvalidPhase;
    return this with { Game = next };
  }

  public Session Declare(Seat seat, IContract declaration)
  {
    var player = GetPlayer(seat);
    var next = Game.Declare(player, declaration) ?? throw Errors.Generic.InvalidPhase;
    return this with { Game = next };
  }

  private static GameContext GetGameContext(FinishedGames finishedGames, Configuration configuration,
    ByPlayer<Seat> activeSeats)
  {
    var needsCompulsory = ByPlayer.Init(
      p =>
          configuration.Session.CompulsorySolos
          && !finishedGames.HasPlayedCompulsorySolo(activeSeats[p])
    );
    return new(needsCompulsory, configuration.Contracts, configuration.Tricks);
  }

  private Player GetPlayer(Seat seat) => AtSeat(seat) ?? throw Errors.Session.SeatPaused;

  public Player? AtSeat(Seat seat)
  {
    return ActiveSeats.Items.FirstOrDefault(t => t.item == seat).player;
  }

  public bool IsFinished => CompleteGames.Games.Count == Configuration.Session.NumberOfGames;
}
