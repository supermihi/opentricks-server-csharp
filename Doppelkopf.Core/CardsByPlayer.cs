global using CardsByPlayer =
  Doppelkopf.Core.Utils.ByPlayer<System.Collections.Immutable.ImmutableArray<Doppelkopf.API.Card>>;

namespace Doppelkopf.Core;

public static class Extensions
{
  public static CardsByPlayer Remove(this CardsByPlayer self, Player player, Card card) =>
    self.Replace(player, self[player].Remove(card));
}
