using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;

namespace Doppelkopf.API;

public interface IGameForPlayer
{
  Task PlayCard(Card card);
  Task DeclareHold(string? holdId);
  Task PlaceBid(Bid bid);
}
