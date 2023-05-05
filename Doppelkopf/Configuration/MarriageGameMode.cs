using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class MarriageGameMode : IGameMode
{
  public GameModeKind Kind => GameModeKind.Marriage;

  public ITrickRules TrickRules { get; } = Tricks.TrickRules.ForTrumpSuit(Suit.Diamonds);
}
