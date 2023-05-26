using Doppelkopf.Errors;
using Doppelkopf.Persistence;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Model;

public static class TableDataExtensions
{
  public static (VersionedSession, Seat) TableAndSeat(this TableData tableData, UserId user)
  {
    if (tableData.Table == null)
    {
      throw Err.Table.NotInitialized;
    }
    var seat = tableData.Users.GetSeat(user);
    if (seat == null)
    {
      throw Error.UserNotAtTable;
    }
    return (tableData.Table, seat.Value);
  }
}
