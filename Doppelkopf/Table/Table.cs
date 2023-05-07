using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.GameFinding;
using Doppelkopf.Scoring;

namespace Doppelkopf.Table;

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
}

public record Table
{
  public IRules Rules { get; }
  public int NumberOfSeats { get; }
  public MatchHistory History { get; init; }

  public Table(IRules rules, int numberOfSeats, Random random)
  {
    Rules = rules;
    NumberOfSeats = numberOfSeats;
    History = MatchHistory.Init(NumberOfSeats, Rules.Rounds.CompulsorySolos);
    var cards = Rules.Deck.Shuffle(random);
    Match = new Match(cards, Auction.Initial, null);
    Seats = GetActiveSeats(0, 0);
  }

  public Match Match { get; init; }
  public ByPlayer<Seat> Seats { get; init; }

  public Table StartNextMatch(Random random)
  {
    if (IsFinished)
    {
      throw Err.Table.StartGame.IsComplete;
    }
    if (!Match.IsFinished)
    {
      throw Err.Table.StartGame.InvalidPhase;
    }
    var isSolo = Match.Contract!.Mode.Kind == GameModeKind.Solo;
    var isCompulsory = isSolo && History.NeedsCompulsorySolo[Seats[Match.Contract.Soloist!.Value]];

    var nextHistory = History.AddMatch(CompletedMatch.FromMatch(Match, Seats!, isCompulsory));

    var cards = Rules.Deck.Shuffle(random);
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
    var newMatch = Match.PlayCard(player, card);
    return this with { Match = newMatch };
  }

  public Table Reserve(Seat seat, bool reserved)
  {
    var player = GetPlayer(seat);
    var newMatch = Match.Reserve(player, reserved, GetMatchContext());
    return this with { Match = newMatch };
  }

  public Table Declare(Seat seat, IGameMode declaration)
  {
    var player = GetPlayer(seat);
    var newMatch = Match.Declare(player, declaration, GetMatchContext());
    return this with { Match = newMatch };
  }

  private MatchContext GetMatchContext() {
    var needsBySeat = History.NeedsCompulsorySolo;
    var needsCompulsory = new ByPlayer<bool>(
        needsBySeat[Seats[Player.Player1]],
        needsBySeat[Seats[Player.Player2]],
        needsBySeat[Seats[Player.Player3]],
        needsBySeat[Seats[Player.Player4]]
    );

    return new(needsCompulsory, Rules.Modes);
  }

  private Player GetPlayer(Seat seat) => AtSeat(seat, Seats) ?? throw Err.Game.PlayCard.SeatPaused;

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
