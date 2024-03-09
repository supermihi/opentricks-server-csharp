using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Sessions;

internal sealed class Session : ISession
{
  private readonly SessionConfiguration _configuration;
  private ByPlayer<bool> _needsCompulsorySolo;
  private readonly IGameFactory _gameFactory;
  private readonly List<GameEvaluation> _completeGames = [];

  public Session(SessionConfiguration configuration, IGameFactory gameFactory)
  {
    if (configuration.NumberOfSeats < Rules.NumPlayers)
    {
      throw new ArgumentException("not enough players");
    }

    _gameFactory = gameFactory;

    _configuration = configuration;
    _needsCompulsorySolo = ByPlayer.Init(configuration.CompulsorySolos);
    Players = Seats.GetActiveSeats(configuration.NumberOfSeats, 0, 0);
    CurrentGame = gameFactory.CreateGame(_needsCompulsorySolo);
  }

  public IGame CurrentGame { get; set; }
  public ByPlayer<Seat> Players { get; set; }

  public void DeclareOk(Seat seat) => CurrentGame.DeclareOk(ActivePlayer(seat));

  public void DeclareHold(Seat seat, IHold hold) => CurrentGame.DeclareHold(ActivePlayer(seat), hold);

  public void PlayCard(Seat seat, Card card)
  {
    var result = CurrentGame.PlayCard(ActivePlayer(seat), card);
    if (result.CompletedGame is not { } evaluation)
    {
      return;
    }

    _completeGames.Add(evaluation);
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
