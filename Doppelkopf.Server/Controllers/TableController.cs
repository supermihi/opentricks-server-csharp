using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Interface;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.Notifications;
using Doppelkopf.Server.Storage;
using Doppelkopf.Server.TableActions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Controllers;

[ApiController]
[Route("table")]
[Authorize]
public class TableController : ControllerBase
{
  private readonly ILogger<TableController> _logger;
  private readonly ITableStore _store;
  private readonly ITableActionListener _tableActionListener;

  public TableController(ILogger<TableController> logger, ITableStore store, ITableActionListener tableActionListener)
  {
    _logger = logger;
    _store = store;
    _tableActionListener = tableActionListener;
  }

  [HttpGet(Name = "GetTables")]
  public IAsyncEnumerable<Table> GetAsync()
  {
    return _store.GetAll(HttpContext.AuthenticatedUser().Id);
  }

  [HttpPost(Name = "CreateTable")]
  public async Task<JsonTable> CreateAsync(JsonCreateTableRequest request)
  {
    var tableId = TableId.New();
    var creator = HttpContext.AuthenticatedUser();
    if (request.MaxSeats is < Constants.NumberOfPlayers or > Constants.MaxSeats)
    {
      throw new ArgumentException("invalid number of seats");
    }
    var rules = new Rules(request.RuleSet, request.MaxSeats);
    var meta = new TableMeta(tableId, DateTime.UtcNow, request.Name, creator.Id, rules);
    var table = Table.Init(meta, request.InvitedUsers);
    await _store.Create(table);
    await _tableActionListener.Notify(new TableActionResult(table, TableEvent.CreateTable(table)));
    _logger.LogInformation("{Creator} created new table {TableId}", creator.Id, tableId);
    return JsonTable.FromTable(table, creator.Id);
  }

  [HttpGet("{id}", Name = "GetTable")]
  [Produces(typeof(Table))]
  public async Task<ActionResult<JsonTable>> GetTableAsync(string id)
  {
    var table = await GetAuthenticatedTable(id);
    return JsonTable.FromTable(table, HttpContext.AuthenticatedUser().Id);
  }

  private async Task<Table> GetAuthenticatedTable(string id)
  {
    var table = await _store.TryGet(new TableId(id));
    if (table is null)
    {
      throw new ArgumentException("not found");
    }
    if (!table.Users.Contains(HttpContext.AuthenticatedUser().Id))
    {
      throw new Exception("forbidden");
    }
    return table;
  }

  [HttpPatch("{id}", Name = "Act")]
  public async Task<UserNotification> ActAsync(string id, TableRequest request)
  {
    var table = await GetAuthenticatedTable(id);
    var user = HttpContext.AuthenticatedUser();
    var result = request.Type switch
    {
      RequestType.JoinTable => table.Join(user.Id),
      RequestType.MarkAsReady => table.MarkReady(user.Id),
      RequestType.Start => table.Start(user.Id),
      RequestType.Reserve => table.Reserve(user.Id, request.IsReserved!.Value),
      RequestType.Declare => table.Declare(user.Id, request.ContractId!),
      RequestType.PlayCard => table.PlayCard(user.Id, request.CardId!),
      _ => throw new ArgumentOutOfRangeException()
    };
    await _store.Update(table, result.Table);
    await _tableActionListener.Notify(result);
    return UserNotification.FromTableActionResult(result, user.Id);
  }
}
