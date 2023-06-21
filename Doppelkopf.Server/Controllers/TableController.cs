using Doppelkopf.API;
using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Interface;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Controllers;

[ApiController]
[Route("table")]
[Authorize]
[MyExceptionFilter]
public class TableController : ControllerBase
{
  private readonly ITableService _tableService;

  public TableController(ITableService tableService)
  {
    _tableService = tableService;
  }

  [HttpGet(Name = "GetTables")]
  public async IAsyncEnumerable<TableState> GetAsync()
  {
    var userId = HttpContext.AuthenticatedUser().Id;
    await foreach (var table in _tableService.GetTables(userId))
    {
      yield return table.ToTableState(userId);
    }
  }

  [HttpPost(Name = "CreateTable")]
  public async Task<TableState> CreateAsync(CreateTableRequest request)
  {
    var creator = HttpContext.AuthenticatedUser();
    var table = await _tableService.Create(
      creator.Id,
      request.Name,
      Rules.FromCreateTableRequest(request),
      request.InvitedUsers?.Select(id => new UserId(id)) ?? Enumerable.Empty<UserId>());

    return table.ToTableState(creator.Id);
  }

  [HttpGet("{id}", Name = "GetTable")]
  [Produces(typeof(Table))]
  public async Task<ActionResult<TableState>> GetTableAsync(string id)
  {
    var (table, user) = await GetAuthenticatedTable(id);
    return table.ToTableState(user);
  }

  private async Task<(Table, UserId)> GetAuthenticatedTable(string id)
  {
    var user = HttpContext.AuthenticatedUser();
    var table = await _tableService.GetTable(new TableId(id), user.Id);
    return (table, user.Id);
  }

  [HttpPatch("{id}", Name = "Act")]
  public async Task<Notification> ActAsync(string id, TableRequest request)
  {
    var (table, user) = await GetAuthenticatedTable(id);
    var result = await _tableService.Act(table, user, request);
    return result.ToNotification(user);
  }

}
