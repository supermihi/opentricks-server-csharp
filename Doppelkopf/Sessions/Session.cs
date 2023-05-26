using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Conf;
using Doppelkopf.Contracts;
using Doppelkopf.Errors;
using Doppelkopf.Games;

namespace Doppelkopf.Sessions;

/// <summary>
/// A session of multiple (24 with tournament rules) Doppelkopf games.
/// </summary>
/// <param name="NumberOfSeats">
/// Number of people participating in this session. Can be larger than <see cref="Constants.NumberOfPlayers"/>; in
/// that case, seats will take turns in being skipped.
/// </param>
/// <param name="ActiveGame">
/// Current game. Is <c>null</c> before the first was started and after the session has ended.
/// </param>
/// <param name="ActiveSeats">
/// <see cref="Seat"/> to <see cref="Player"/> mapping for the current game.
/// </param>
public sealed record Session(Configuration Configuration, int NumberOfSeats, GameHistory CompletedGames,
  Game? ActiveGame, ByPlayer<Seat> ActiveSeats)
{
  public Session(Configuration configuration, int numberOfSeats = 4) : this(
    configuration,
    numberOfSeats,
    GameHistory.Empty,
    null,
    Seats.GetActiveSeats(numberOfSeats, 0, 0))
  { }

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
    var newGame = Game.Init(GetCurrentGameContext(), cards);
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
    var next = ActiveGame?.Reserve(player, reserved) ?? throw Err.Auction.Reserve.InvalidPhase;
    return this with { ActiveGame = next };
  }

  public Session Declare(Seat seat, IContract declaration)
  {
    var player = GetPlayer(seat);
    var next = ActiveGame?.Declare(player, declaration) ?? throw Err.Auction.Declare.InvalidPhase;
    return this with { ActiveGame = next };
  }

  private GameContext GetCurrentGameContext()
  {
    var needsCompulsory = ByPlayer.Init(
      p =>
          Configuration.Session.CompulsorySolos
          && !CompletedGames.HasPlayedCompulsorySolo(ActiveSeats[p])
    );
    return new(needsCompulsory, Configuration.Contracts, Configuration.Tricks);
  }

  private Player GetPlayer(Seat seat) => AtSeat(seat, ActiveSeats) ?? throw Err.TrickTaking.PlayCard.SeatPaused;

  private static Player? AtSeat(Seat seat, ByPlayer<Seat> seats)
  {
    return seats.Items.FirstOrDefault(t => t.item == seat).player;
  }

  public bool IsFinished => CompletedGames.Games.Count == Configuration.Session.NumberOfGames;
}
