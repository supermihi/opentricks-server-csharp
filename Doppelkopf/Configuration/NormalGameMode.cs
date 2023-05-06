using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class NormalGameMode : IGameMode
{
  public NormalGameMode(EldersMode elders)
  {
    TrickRules = Tricks.TrickRules.ForTrumpSuit(Suit.Diamonds, elders);
  }

  public GameModeKind Kind => GameModeKind.NormalGame;
  public ITrickRules TrickRules { get; }
}
