using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Games;

public interface IGame
{
  Modes Modes { get; }
  void DeclareHold(Player player, IHold hold);
  void DeclareOk(Player player);
  PlayCardResult PlayCard(Player player, Card card);
  void PlaceBid(Player player, Bid bid);
  GamePhase Phase { get; }
  CardsByPlayer Cards { get; }
  Player? Turn { get; }
  IEnumerable<Declaration> Declarations { get; }
  AuctionResult? AuctionResult { get; }
  IReadOnlyList<CompleteTrick> CompleteTricks { get; }
  Trick? CurrentTrick { get; }
  GameEvaluation Evaluate();
}
