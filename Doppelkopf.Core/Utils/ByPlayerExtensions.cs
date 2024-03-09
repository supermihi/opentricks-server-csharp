namespace Doppelkopf.Core.Utils;

public static class ByPlayerExtensions
{
  public static IEnumerable<T> InOrder<T>(this ByPlayer<T> self, Player start) =>
    start.Cycle().Take(Rules.NumPlayers).Select(p => self[p]);
}
