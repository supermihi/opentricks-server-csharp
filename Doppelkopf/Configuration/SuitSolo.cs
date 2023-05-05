using Doppelkopf.Cards;
using Doppelkopf.Tricks;

namespace Doppelkopf.Configuration;

public record SuitSolo(Suit Suit) : IGameMode {
  public GameModeKind Kind => GameModeKind.Solo;

  public ITrickRules TrickRules => Tricks.TrickRules.ForTrumpSuit(Suit);
}