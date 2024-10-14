using System.Collections.Immutable;

namespace Doppelkopf.Service;

public sealed record TableData
{
  public TableId Id { get; }
  public string Name { get; }
  private readonly ImmutableArray<UserId> _usersWithOwnerFirst;
  public UserId Owner => _usersWithOwnerFirst[0];
  public IReadOnlyList<UserId> Members => _usersWithOwnerFirst;

  public TableData(TableId id, string name, UserId owner)
  {
    Id = id;
    Name = name;
    _usersWithOwnerFirst = ImmutableArray.Create(owner);
  }
}
