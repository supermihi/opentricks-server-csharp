using Doppelkopf.Server.Model;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Tests.TableTests;

public class TableAddUserTests
{
  [Fact]
  public void ForbidsAddUserIfTableIsStarted()
  {
    var table = TestData.Table.JustStarted;
    Assert.True(table.IsStarted);
    var exc = Assert.Throws<UserInputException>(() => table.AddUser(TestData.Users.NotMember, ready: true));
    Assert.Equal("table_started", exc.ErrorCode);
  }

  [Fact]
  public void ForbidsAddUserIfAlreadyJoined()
  {
    var table = TestData.Table
        .Initial
        .AddUser(TestData.Users.Member1, ready: true)
        .Table;
    MyAssert.ThrowsUserError("already_at_table", () => table.AddUser(TestData.Users.Member1, ready: true));
  }

  [Fact]
  public void ForbidsAddUserIfAlreadyFull()
  {
    var table = TestData.Table
        .Initial
        .AddUser(TestData.Users.Member1, true)
        .Table
        .AddUser(TestData.Users.Member2, true)
        .Table
        .AddUser(TestData.Users.Member3, true)
        .Table;
    MyAssert.ThrowsUserError("table_full", () => table.AddUser(TestData.Users.NotMember, ready: true));
  }
}
