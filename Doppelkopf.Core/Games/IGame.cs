using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Games;

public interface IGame
{
  GameConfiguration Configuration { get; }
  void Play(Player player, PlayerAction action);
  GamePhase Phase { get; }
  CardsByPlayer Cards { get; }
  Player? GetTurn();
  IEnumerable<Declaration> Declarations { get; }
  AuctionResult? AuctionResult { get; }
  IReadOnlyList<CompleteTrick> CompleteTricks { get; }
  Trick? CurrentTrick { get; }
  GameEvaluation Evaluate();
  int Age { get; }
}
