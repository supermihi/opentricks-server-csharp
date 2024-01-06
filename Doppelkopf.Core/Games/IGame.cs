using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Scoring;

namespace Doppelkopf.Core.Games;

public interface IGame
{
  void MakeReservation(Player player, IDeclarableContract contract);
  void DeclareOk(Player player);
  PlayCardResult PlayCard(Player player, Card card);
  void PlaceBid(Player player, Bid bid);
  GamePhase Phase { get; }
}
