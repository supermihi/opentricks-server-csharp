namespace Doppelkopf.Server.Tests.TableTests;

public class TableStartTests
{
  [Fact]
  public void RefusesToStartTableWithTooLittleUsers()
  {
    var table = TestData.Table.Initial;
    MyAssert.ThrowsUserError("too_few_users", () => table.Start(table.Meta.Owner));
  }
}
