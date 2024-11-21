using System.Diagnostics.CodeAnalysis;
using Doppelkopf.API;
using Doppelkopf.API.Errors;
using Doppelkopf.Core;
using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Sessions;

internal sealed record CompleteGame(GameEvaluation Evaluation, AuctionResult AuctionResult);
internal sealed class Session : ISession
{
  private readonly SessionConfiguration _configuration;
  private readonly IGameFactory _gameFactory;
  private readonly List<CompleteGame> _completeGames = [];

  public Session(SessionConfiguration configuration, IGameFactory gameFactory)
  {
    if (configuration.NumberOfSeats < Rules.NumPlayers)
    {
      throw new ArgumentException("not enough players");
    }

    _gameFactory = gameFactory;

    _configuration = configuration;
    StartNextGame();
  }

  private ByPlayer<bool> NeedCompulsorySolos()
  {
    return ByPlayer.Init(p =>
      _configuration.CompulsorySolos &&
      !_completeGames.Any(game => game.AuctionResult is { IsCompulsorySolo: true, Declarer: { } declarer } && declarer == p));
  }

  public IGame CurrentGame { get; set; }
  public ByPlayer<Seat> Players { get; set; }

  public void Play(Seat seat, PlayerAction action)
  {
    CurrentGame.Play(ActivePlayer(seat), action);
    if (CurrentGame.Phase == GamePhase.Finished)
    {
      CompleteGame();
    }
  }

  private void CompleteGame()
  {
    var evaluation = CurrentGame.Evaluate();
    _completeGames.Add(new(evaluation, CurrentGame.AuctionResult!));
    if (_completeGames.Count < _configuration.NumberOfGames)
    {
      StartNextGame();
    }
  }

  [MemberNotNull(nameof(Players))]
  [MemberNotNull(nameof(CurrentGame))]
  private void StartNextGame()
  {
    Players = Seats.GetActiveSeats(_configuration.NumberOfSeats, _completeGames.Count, 0 /* TODO */);
    CurrentGame = _gameFactory.CreateGame(NeedCompulsorySolos());
  }

  public Player ActivePlayer(Seat seat) => AtSeat(seat) ?? throw ErrorCodes.SeatPaused.ToException();

  private Player? AtSeat(Seat seat)
  {
    if (seat.Position >= _configuration.NumberOfSeats)
    {
      throw ErrorCodes.InvalidSeat.ToException();
    }

    return Players.Items.FirstOrDefault(t => t.value == seat).player;
  }
}
