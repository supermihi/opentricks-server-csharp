using Doppelkopf.Sessions;
using Doppelkopf.Users.API;

namespace Doppelkopf.Service;

public static class TableDataExtensions
{
  public static Seat SeatOf(this TableData data, UserId user)
  {
    for (var i = 0; i < data.Members.Count; ++i)
    {
      if (user == data.Members[i])
      {
        return new Seat(i);
      }
    }
    throw new ArgumentException("specified user not at this table");
  }
}