using Doppelkopf.API.Errors;

namespace Doppelkopf.Service;

public static class ErrorCodes
{
  public static readonly ErrorCode NotOwner = new("not_table_owner", "you are not owner of the table");
  public static readonly ErrorCode NotReady = new("not_all_ready", "not all table members are ready to start");
  public static readonly ErrorCode TableStarted = new("table_started", "the table is already started");
  public static readonly ErrorCode TableNotStarted = new("table_not_started", "the table is not yet started");
  public static readonly ErrorCode NotMember = new("not_member", "you are not a member of this table");
}
