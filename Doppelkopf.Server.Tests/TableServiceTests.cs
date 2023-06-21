using Doppelkopf.API;
using Doppelkopf.Server.Controllers;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.Notifications;
using Doppelkopf.Server.Storage;
using Moq;

namespace Doppelkopf.Server.Tests;

public class TableServiceTests
{
  [Fact]
  public async Task AllowsGetTableOnlyForMembers()
  {
    var owner = new UserId("owner");
    var member = new UserId("member");
    var meta = TableMeta.Create("table", owner, new Rules(RuleSet.Minikopf, 4));
    var table = Table.Init(meta, new[] {member});
    var tableStore = new Mock<ITableStore>(MockBehavior.Strict);
    tableStore.Setup(t => t.TryGet(meta.Id)).ReturnsAsync(table);
    var service = new TableService(tableStore.Object, new NotificationDispatcher());
    Assert.Equal(table, await service.GetTable(meta.Id, owner));
    Assert.Equal(table, await service.GetTable(meta.Id, member));
    await MyAssert.ThrowsUserErrorAsync("not_allowed", () => service.GetTable(meta.Id, new("other")));
  }

  [Fact]
  public async Task ThrowsIfNotFound()
  {
    var tableStore = new Mock<ITableStore>(MockBehavior.Loose);
    var service = new TableService(tableStore.Object, new NotificationDispatcher());
    await MyAssert.ThrowsUserErrorAsync("not_found", () => service.GetTable(new("table"), new("user")));
  }
}
