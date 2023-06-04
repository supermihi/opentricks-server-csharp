using Doppelkopf.API;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.TableActions;

public static class TableExtensions
{
  public static UserId UserForPlayer(this Table table, Player player)
  {
    var seat = table.Session!.ActiveSeats[player];
    return table.Users[seat];
  }
}
