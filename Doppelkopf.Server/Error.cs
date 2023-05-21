using Doppelkopf.Errors;

namespace Doppelkopf.Server;

public static class Error {
  public static readonly InputException UserNotAtTable =
      new(Components.Table, "generic", "not_at_table", "you are not a member of this table");
}