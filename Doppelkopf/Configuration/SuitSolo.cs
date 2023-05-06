using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public class SuitSolo : IGameMode
{
  public Suit Trump { get; }

  public SuitSolo(Suit trump, EldersMode elders)
  {
    Trump = trump;
    TrickRules = Tricks.TrickRules.ForTrumpSuit(trump, elders);
  }

  public GameModeKind Kind => GameModeKind.Solo;

  public ITrickRules TrickRules { get; }
}
