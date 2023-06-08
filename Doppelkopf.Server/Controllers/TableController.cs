using Doppelkopf.API;
using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Bots;
using Doppelkopf.Server.Interface;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.Notifications;
using Doppelkopf.Server.Storage;
using Doppelkopf.Server.TableActions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Controllers;

public interface ITableService
{
  IAsyncEnumerable<Table> GetTables(UserId userId);
  Task<Table> Create(UserId creator, string tableName, Rules rules, IEnumerable<UserId> invitedUsers);
  Task<Table> GetTable(TableId id, UserId client);
  Task<TableActionResult> Act(Table table, UserId user, TableRequest request);
}

public class TableService : ITableService
{
  private readonly ITableStore _store;
  private readonly NotificationDispatcher _dispatcher;
  private readonly ILogger<TableService>? _logger;
  private readonly BotIds? _botIds;

  public TableService(ITableStore store, NotificationDispatcher dispatcher, BotIds? botIds = null,
    ILogger<TableService>? logger = null)
  {
    _store = store;
    _dispatcher = dispatcher;
    _botIds = botIds;
    _logger = logger;
  }

  public async Task<Table> GetTable(TableId id, UserId client)
  {
    var table = await _store.TryGet(new TableId(id));
    if (table is null)
    {
      throw new ArgumentException("not found");
    }
    if (!table.Users.Contains(client))
    {
      throw new Exception("forbidden");
    }
    return table;
  }

  public async Task<Table> Create(UserId creator, string tableName, Rules rules, IEnumerable<UserId> invitedUsers)
  {
    var tableId = TableId.New();
    if (rules.MaxSeats is < Constants.NumberOfPlayers or > Constants.MaxSeats)
    {
      throw new ArgumentException("invalid number of seats");
    }
    var meta = new TableMeta(tableId, DateTime.UtcNow, tableName, creator, rules);
    var table = Table.Init(meta, invitedUsers.Select(s => new UserId(s)));
    await _store.Create(table);
    await _dispatcher.Notify(new TableActionResult(table, TableEvent.CreateTable(table)));
    _logger?.LogInformation("{Creator} created new table {TableId}", creator.Id, tableId);
    return table;
  }

  public IAsyncEnumerable<Table> GetTables(UserId userId) => _store.GetAll(userId);

  public async Task<TableActionResult> Act(Table table, UserId userId, TableRequest request)
  {
    var result = request.Type switch
    {
      RequestType.JoinTable => table.AddUser(userId, false),
      RequestType.AddBot => AddBot(table),
      RequestType.MarkAsReady => table.MarkReady(userId),
      RequestType.Start => table.Start(userId),
      RequestType.Reserve => table.Reserve(userId, request.IsReserved!.Value),
      RequestType.Declare => table.Declare(userId, request.ContractId!),
      RequestType.PlayCard => table.PlayCard(userId, request.CardId!),
      _ => throw new ArgumentOutOfRangeException()
    };
    await _store.Update(table, result.Table);
    await _dispatcher.Notify(result);
    return result;
  }

  private TableActionResult AddBot(Table table)
  {
    var idOrNull = _botIds?.Bots.FirstOrDefault(botId => !table.Users.Contains(botId));
    return idOrNull is { } id ? table.AddUser(id, true) : throw new ArgumentException("no bot available");
  }
}

[ApiController]
[Route("table")]
[Authorize]
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
  public async Task<UserNotification> ActAsync(string id, TableRequest request)
  {
    var (table, user) = await GetAuthenticatedTable(id);
    var result = await _tableService.Act(table, user, request);
    return UserNotification.FromTableActionResult(result, user);
  }

}
