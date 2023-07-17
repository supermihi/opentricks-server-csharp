using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Sessions;

public class Session
{
  private readonly SessionConfiguration _configuration;
  private readonly int _numberOfSeats;
  private ByPlayer<bool> _needsCompulsorySolo;

  public Session(SessionConfiguration configuration, IGameFactory gameFactory, int numberOfSeats)
  {
    _configuration = configuration;
    _numberOfSeats = numberOfSeats;
    _needsCompulsorySolo = ByPlayer.Init(configuration.CompulsorySolos);
    Players = Seats.GetActiveSeats(numberOfSeats, 0, 0);
    CurrentGame = gameFactory.CreateGame(_needsCompulsorySolo);
  }

  public IGame CurrentGame { get; set; }
  public ByPlayer<Seat> Players { get; set; }

  public void DeclareOk(Seat seat) => CurrentGame.DeclareOk(ActivePlayer(seat));

  public void MakeReservation(Seat seat, IDeclarableContract contract) =>
      CurrentGame.MakeReservation(ActivePlayer(seat), contract);

  public void PlayCard(Seat seat, Card card) => CurrentGame.PlayCard(ActivePlayer(seat), card);

  private Player ActivePlayer(Seat seat) => AtSeat(seat) ?? throw ErrorCodes.SeatPaused.ToException();

  public Player? AtSeat(Seat seat)
  {
    if (seat.Position >= _numberOfSeats)
    {
      throw ErrorCodes.InvalidSeat.ToException();
    }
    return Players.Items.FirstOrDefault(t => t.item == seat).player;
  }
}
