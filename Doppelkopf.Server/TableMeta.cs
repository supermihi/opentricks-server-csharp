using System.Collections.Immutable;
using Doppelkopf.Tables;

namespace Doppelkopf.Server;

public sealed record TableMeta(TableId Id, IImmutableList<UserId> Players, UserId Owner)
{
  public static TableMeta Init(TableId id, UserId owner) =>
      new(id, ImmutableArray.Create(owner), owner);

  public Seat? GetSeat(UserId user)
  {
    var index = Players.IndexOf(user);
    if (index == -1)
    {
      return null;
    }
    return new Seat(index);
  }

  public bool ContainsUser(UserId user) => Players.Contains(user);
}