namespace Doppelkopf.Server;

public static class ServerErrors
{
  public static readonly InputException UserNotAtTable =
      new("not_at_table", "you are not a member of this table");
}
