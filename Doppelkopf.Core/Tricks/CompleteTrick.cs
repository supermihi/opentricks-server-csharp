using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks;

/// <summary>
/// A complete trick with the winner evaluated.
/// </summary>
/// <param name="Leader">Leader of the trick.</param>
/// <param name="Winner">Winner of the trick.</param>
/// <param name="Cards">The cards, by player.</param>
/// <param name="Index">Zero-based index of the trick.</param>
/// <param name="Remaining">Number of tricks to follow after this one.</param>
public sealed record CompleteTrick(Player Leader, Player Winner, ByPlayer<Card> Cards, int Index, int Remaining)
{
  public Card WinningCard => Cards[Winner];
}
