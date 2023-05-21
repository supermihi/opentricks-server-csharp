using System.Collections;
using System.Collections.Immutable;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Model;

public readonly record struct TableUsers(IImmutableList<UserId> Users) : IReadOnlyList<UserId>
{
  public TableUsers Add(UserId user, Random? rnd = null)
  {
    if (Users.Contains(user))
    {
      throw new Exception("user already at table");
    }
    var position = (rnd ?? Random.Shared).Next(0, Users.Count + 1);
    return new(Users.Insert(position, user));
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public IEnumerator<UserId> GetEnumerator() => Users.GetEnumerator();

  public int Count => Users.Count;
  public UserId this[int index] => Users[index];
  public Seat? GetSeat(UserId user) {
    var index = Users.IndexOf(user);
    return index == -1 ? null : new Seat(Users.IndexOf(user));
  }
}
