using Doppelkopf.Cards;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.Storage;
using Moq;

namespace Doppelkopf.Server.Tests;

public class Pers
{
  [Fact]
  public async Task Test1()
  {
    var owner = new UserId("michael");
    var tableMeta = TableMeta.Init(TableId.New(), "test", owner);
    var tableData = TableData.Init(tableMeta);
    var table = new PersistingAndPublishingTable(tableData, Mock.Of<ITableStore>());

    var user = Assert.Single(table.Data.Users);
    Assert.Equal(owner, user);

    var nextUser = new UserId("gorm");
    await table.AddUser(nextUser);

    Assert.Equal(2, table.Data.Users.Count);

    await table.PlayCard(owner, Card.QueenOfClubs);
  }
}
