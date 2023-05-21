using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.GameFinding;
using Doppelkopf.Games;

namespace Doppelkopf.Sessions;

/// <summary>
/// A session of multiple (24 with tournament rules) Doppelkopf games.
/// </summary>
public sealed record Session
{
  public IConfiguration Configuration { get; }

  /// <summary>
  /// Number of people participating in this session. Can be larger than <see cref="Constants.NumberOfPlayers"/>; in
  /// that case, seats will take turns in being skipped.
  /// </summary>
  public int NumberOfSeats { get; }
  public GameHistory CompletedGames { get; private init; }

  /// <summary>
  /// Current game. Is <c>null</c> before the first was started and after the session has ended.
  /// </summary>
  public Game? ActiveGame { get; private init; }

  /// <summary>
  /// <see cref="Seat"/> to <see cref="Player"/> mapping for the current game.
  /// </summary>
  public ByPlayer<Seat> ActiveSeats { get; private init; }

  public Session(IConfiguration configuration, int numberOfSeats)
  {
    Configuration = configuration;
    NumberOfSeats = numberOfSeats;
    CompletedGames = GameHistory.Init(NumberOfSeats, Configuration.Rounds.CompulsorySolos);
    ActiveSeats = Seats.GetActiveSeats(NumberOfSeats, 0, 0);
  }

  public Session StartNextGame(ByPlayer<IImmutableList<Card>> cards)
  {
    if (IsFinished)
    {
      throw Err.Table.StartGame.IsComplete;
    }
    if (ActiveGame is null || !ActiveGame.IsFinished)
    {
      throw Err.Table.StartGame.InvalidPhase;
    }
    var newHistory = CompletedGames.AddGame(ActiveGame, ActiveSeats);
    var newGame = new Game(cards, Auction.Initial, null);
    var newSeats = newHistory.NextGameActiveSeats(NumberOfSeats);
    return this with { ActiveGame = newGame, ActiveSeats = newSeats, CompletedGames = newHistory };
  }

  public Session PlayCard(Seat seat, Card card)
  {
    var player = GetPlayer(seat);
    var newMatch =
      ActiveGame?.PlayCard(player, card) ?? throw Err.TrickTaking.PlayCard.InvalidPhase;
    return this with { ActiveGame = newMatch };
  }

  public Session Reserve(Seat seat, bool reserved)
  {
    var player = GetPlayer(seat);
    var next =
      ActiveGame?.Reserve(player, reserved, GetMatchContext())
      ?? throw Err.Auction.Reserve.InvalidPhase;
    return this with { ActiveGame = next };
  }

  public Session Declare(Seat seat, IGameMode declaration)
  {
    var player = GetPlayer(seat);
    var next =
      ActiveGame?.Declare(player, declaration, GetMatchContext())
      ?? throw Err.Auction.Declare.InvalidPhase;
    return this with { ActiveGame = next };
  }

  private GameContext GetMatchContext()
  {
    var needsBySeat = CompletedGames.NeedsCompulsorySolo;
    var needsCompulsory = new ByPlayer<bool>(
      needsBySeat[ActiveSeats[Player.Player1]],
      needsBySeat[ActiveSeats[Player.Player2]],
      needsBySeat[ActiveSeats[Player.Player3]],
      needsBySeat[ActiveSeats[Player.Player4]]
    );

    return new(needsCompulsory, Configuration.Modes);
  }

  private Player GetPlayer(Seat seat) =>
    AtSeat(seat, ActiveSeats) ?? throw Err.TrickTaking.PlayCard.SeatPaused;

  private static Player? AtSeat(Seat seat, ByPlayer<Seat> seats)
  {
    return seats.Items.FirstOrDefault(t => t.item == seat).player;
  }

  public bool IsFinished => CompletedGames.Games.Count == Configuration.Rounds.NumberOfGames;
}
