namespace Doppelkopf.Server.Model;

public sealed record TableMeta(TableId Id, DateTime CreatedUtc, string Name, UserId Owner, Rules Rules);
