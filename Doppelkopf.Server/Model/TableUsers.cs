using System.Collections;
using System.Collections.Immutable;
using Doppelkopf.API;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Model;

public sealed class TableUsers : IReadOnlyCollection<UserId>
{
  private readonly IImmutableList<(UserId user, bool ready)> _data;

  private TableUsers(IImmutableList<(UserId user, bool ready)> data)
  {
    var duplicateEntry = data.FirstOrDefault(userAndReay => data.Count(other => userAndReay.user == other.user) > 1);
    if (duplicateEntry != default)
    {
      throw new ArgumentException($"duplicate user id {duplicateEntry.user}");
    }
    _data = data;
  }

  private (UserId user, bool ready)? Find(UserId user)
  {
    foreach (var entry in _data)
    {
      if (entry.user == user)
      {
        return entry;
      }
    }
    return null;
  }

  public static TableUsers Init(IEnumerable<UserId> users) => new(users.Select(u => (u, false)).ToImmutableArray());
  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public IEnumerator<UserId> GetEnumerator() => _data.Select(userAndReady => userAndReady.user).GetEnumerator();
  public int Count => _data.Count;
  public TableUsers Add(UserId user) => new(_data.Add((user, false)));

  public bool IsReady(UserId user)
  {
    return Find(user)?.ready ?? throw new KeyNotFoundException("user not in TableUsers");
  }

  public TableUsers SetReady(UserId user) => new(_data.Replace((user, false), (user, true)));
  public bool AreAllReady => _data.All(t => t.ready);

  public Seat SeatOf(UserId user)
  {
    for (var i = 0; i < _data.Count; ++i)
    {
      if (_data[i].user == user)
      {
        return new(i);
      }
    }
    throw new KeyNotFoundException("user not at table");
  }

  public UserId this[Seat seat]
  {
    get { return _data[seat.Position].user; }
  }
}
