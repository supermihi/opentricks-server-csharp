using Doppelkopf.Cards;
using Doppelkopf.Configuration;

namespace Doppelkopf.Tricks;

public interface ITrickRules {
  public CardComparison Compare(Card current, Card best);
  bool SameTrickSuit(Card a, Card b);
}