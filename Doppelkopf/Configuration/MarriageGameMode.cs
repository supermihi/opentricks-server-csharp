using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class MarriageGameMode : IGameMode
{
  public MarriageGameMode(EldersMode elders)
  {
    TrickRules = Tricks.TrickRules.ForTrumpSuit(Suit.Diamonds, elders);
  }

  public GameModeKind Kind => GameModeKind.Marriage;

  public ITrickRules TrickRules { get; }
}
