using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class NormalGameMode : IGameMode
{
  public GameModeKind Kind => GameModeKind.NormalGame;
  public ITrickRules TrickRules { get; } = Tricks.TrickRules.ForTrumpSuit(Suit.Diamonds);
}
