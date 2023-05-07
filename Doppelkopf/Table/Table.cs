using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.GameFinding;
using Doppelkopf.Scoring;

namespace Doppelkopf.Table;

public class Table
{
  private readonly IRules _rules;
  private readonly int _numberOfPlayers;

  public IImmutableList<CompletedMatch> CompletedMatches { get; private set; } =
    ImmutableList<CompletedMatch>.Empty;

  public record MatchState(Match Match, ByPlayer<Seat> Players);

  public MatchState? CurrentMatch { get; private set; } = null;

  public Table(IRules rules, int numberOfPlayers)
  {
    _rules = rules;
    _numberOfPlayers = numberOfPlayers;
  }

  public MatchState StartNextGame(Random random)
  {
    if (IsFinished)
    {
      throw Err.Table.StartGame.IsComplete;
    }
    if (CurrentMatch != null)
    {
      throw Err.Table.StartGame.InvalidPhase;
    }
    var players = GetActivePlayers(
      CompletedMatches.Count,
      CompletedMatches.Count(g => g.IsCompulsorySolo)
    );
    var cards = _rules.Deck.Shuffle(random);
    CurrentMatch = new(new(cards, Auction.Initial, null), players);
    return CurrentMatch;
  }

  public bool IsFinished => CompletedMatches.Count == _rules.Rounds.NumberOfGames;

  public ByPlayer<Seat> GetActivePlayers(int gamesPlayed, int dealingsRepeated)
  {
    var dealerIndex = gamesPlayed - dealingsRepeated;
    var numSkipped = _numberOfPlayers - Constants.NumberOfPlayers;
    var player1 = new Seat((dealerIndex + numSkipped) % Constants.NumberOfPlayers);
    var player2 = player1.Next(_numberOfPlayers);
    var player3 = player2.Next(_numberOfPlayers);
    var player4 = player3.Next(_numberOfPlayers);
    return new ByPlayer<Seat>(player1, player2, player3, player4);
  }
}
