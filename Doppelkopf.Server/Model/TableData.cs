using System.Collections.Immutable;
using Doppelkopf.Persistence;

namespace Doppelkopf.Server.Model;

public sealed record TableData(VersionedTable? Table, TableMeta Meta, TableUsers Users)
{
    public static TableData Init(TableMeta meta)
    {
        return new(null, meta, new(ImmutableList.Create<UserId>(meta.Owner)));
    }

    public TableData AddUser(UserId user, Random? random)
    {
        if (IsStarted)
        {
            throw new Exception("already started");
        }
        return this with { Users = Users.Add(user, random) };
    }

    public bool IsStarted => Table != null;
}
