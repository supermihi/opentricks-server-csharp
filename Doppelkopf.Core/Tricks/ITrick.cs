using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks;

public interface ITrick
{
  Player Leader { get; }
  Player? Winner { get; }
  InTurns<Card> Cards { get; }
}
