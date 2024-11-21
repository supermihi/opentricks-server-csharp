namespace Doppelkopf.API.Views;

/// <summary>
/// View of a single trick.
/// </summary>
/// <param name="Leader">The leader (starting player) of the trick.</param>
/// <param name="Cards">Played cards (length 0 to 4).</param>
/// <param name="Index">Index of the trick (zero-based).</param>
/// <param name="Winner">Winner of the trick; <c>null</c> for in-progress tricks.</param>
public sealed record TrickView(Player Leader, IReadOnlyList<Card> Cards, int Index, Player? Winner);
