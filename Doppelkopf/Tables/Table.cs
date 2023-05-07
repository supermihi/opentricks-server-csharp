using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.GameFinding;
using Doppelkopf.Scoring;

namespace Doppelkopf.Tables;

public record MatchHistory(
  IImmutableList<CompletedMatch> Matches,
  IImmutableDictionary<Seat, bool> NeedsCompulsorySolo
)
{
  public static MatchHistory Init(int numberOfSeats, bool compulsorySolos)
  {
    return new(
      ImmutableArray<CompletedMatch>.Empty,
      Enumerable
        .Range(0, numberOfSeats)
        .ToImmutableDictionary(s => new Seat(s), s => compulsorySolos)
    );
  }

  public MatchHistory AddMatch(CompletedMatch match)
  {
    return new(
      Matches.Add(match),
      match.IsCompulsorySolo
        ? NeedsCompulsorySolo.SetItem(match.Players[match.Contract.Soloist!.Value], false)
        : NeedsCompulsorySolo
    );
  }

  public MatchHistory AddMatch(Match match, ByPlayer<Seat> seats)
  {
    if (match.TrickTaking?.IsFinished ?? true)
    {
      throw new IllegalStateException("cannot create completed match from unfinished match");
    }
    var isSolo = match.Contract!.Mode.Kind == GameModeKind.Solo;
    var isCompulsory = isSolo && NeedsCompulsorySolo[seats[match.Contract!.Soloist!.Value]];

    var completedMatch = new CompletedMatch(
      match.TrickTaking.Contract,
      isCompulsory,
      seats,
      new ByPlayer<int>(0, 0, 0, 0) // TODO
    );
    return AddMatch(completedMatch);
  }
}

public record Table
{
  public IRules Rules { get; }
  public int NumberOfSeats { get; }
  public MatchHistory History { get; init; }

  public Table(IRules rules, int numberOfSeats)
  {
    Rules = rules;
    NumberOfSeats = numberOfSeats;
    History = MatchHistory.Init(NumberOfSeats, Rules.Rounds.CompulsorySolos);
    Seats = GetActiveSeats(0, 0);
  }

  public Match? Match { get; init; }
  public ByPlayer<Seat> Seats { get; init; }

  public Table StartNextMatch(ByPlayer<IImmutableList<Card>> cards)
  {
    if (IsFinished)
    {
      throw Err.Table.StartGame.IsComplete;
    }
    if (Match is null || !Match.IsFinished)
    {
      throw Err.Table.StartGame.InvalidPhase;
    }
    var nextHistory = History.AddMatch(Match, Seats);
    var newMatch = new Match(cards, Auction.Initial, null);
    var seats = GetActiveSeats(
      nextHistory.Matches.Count,
      nextHistory.Matches.Count(g => g.IsCompulsorySolo)
    );
    return this with { Match = newMatch, Seats = seats, History = nextHistory };
  }

  public Table PlayCard(Seat seat, Card card)
  {
    var player = GetPlayer(seat);
    var newMatch = Match?.PlayCard(player, card) ?? throw Err.TrickTaking.PlayCard.InvalidPhase;
    return this with { Match = newMatch };
  }

  public Table Reserve(Seat seat, bool reserved)
  {
    var player = GetPlayer(seat);
    var next =
      Match?.Reserve(player, reserved, GetMatchContext()) ?? throw Err.Auction.Reserve.InvalidPhase;
    return this with { Match = next };
  }

  public Table Declare(Seat seat, IGameMode declaration)
  {
    var player = GetPlayer(seat);
    var next =
      Match?.Declare(player, declaration, GetMatchContext())
      ?? throw Err.Auction.Declare.InvalidPhase;
    return this with { Match = next };
  }

  private MatchContext GetMatchContext()
  {
    var needsBySeat = History.NeedsCompulsorySolo;
    var needsCompulsory = new ByPlayer<bool>(
      needsBySeat[Seats[Player.Player1]],
      needsBySeat[Seats[Player.Player2]],
      needsBySeat[Seats[Player.Player3]],
      needsBySeat[Seats[Player.Player4]]
    );

    return new(needsCompulsory, Rules.Modes);
  }

  private Player GetPlayer(Seat seat) =>
    AtSeat(seat, Seats) ?? throw Err.TrickTaking.PlayCard.SeatPaused;

  private static Player? AtSeat(Seat seat, ByPlayer<Seat> seats)
  {
    return seats.Items.FirstOrDefault(t => t.item == seat).player;
  }

  public bool IsFinished => History.Matches.Count == Rules.Rounds.NumberOfGames;

  public ByPlayer<Seat> GetActiveSeats(int gamesPlayed, int dealingsRepeated)
  {
    var dealerIndex = gamesPlayed - dealingsRepeated;
    var numSkipped = NumberOfSeats - Constants.NumberOfPlayers;
    var player1 = new Seat((dealerIndex + numSkipped) % Constants.NumberOfPlayers);
    var player2 = player1.Next(NumberOfSeats);
    var player3 = player2.Next(NumberOfSeats);
    var player4 = player3.Next(NumberOfSeats);
    return new ByPlayer<Seat>(player1, player2, player3, player4);
  }
}
