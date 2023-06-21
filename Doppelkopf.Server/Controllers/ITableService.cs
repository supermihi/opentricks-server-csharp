using Doppelkopf.API;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Controllers;

public interface ITableService
{
  IAsyncEnumerable<Table> GetTables(UserId userId);
  Task<Table> Create(UserId creator, string tableName, Rules rules, IEnumerable<UserId> invitedUsers);
  Task<Table> GetTable(TableId id, UserId client);
  Task<TableActionResult> Act(Table table, UserId user, TableRequest request);
}
