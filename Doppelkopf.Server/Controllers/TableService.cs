using System.Net;
using Doppelkopf.API;
using Doppelkopf.Server.Bots;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.Notifications;
using Doppelkopf.Server.Storage;
using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Controllers;

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
      throw new UserInputException(HttpStatusCode.NotFound, "not_found", "the specified table was not found");
    }
    if (!table.Users.Contains(client))
    {
      throw new UserInputException(HttpStatusCode.Forbidden, "not_allowed", "you are not a member of this table");
    }
    return table;
  }

  public async Task<Table> Create(UserId creator, string tableName, Rules rules, IEnumerable<UserId> invitedUsers)
  {
    if (rules.MaxSeats is < Constants.NumberOfPlayers or > Constants.MaxSeats)
    {
      throw new ArgumentException("invalid number of seats");
    }
    var meta = TableMeta.Create(tableName, creator, rules);
    var table = Table.Init(meta, invitedUsers.Select(s => new UserId(s)));
    await _store.Create(table);
    await _dispatcher.Notify(new TableActionResult(table, new CreateTableEvent(table)), creator);
    _logger?.LogInformation("{Creator} created new table {TableId}", creator.Id, meta.Id);
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
    await _dispatcher.Notify(result, userId);
    return result;
  }

  private TableActionResult AddBot(Table table)
  {
    var idOrNull = _botIds?.Bots.FirstOrDefault(botId => !table.Users.Contains(botId));
    return idOrNull is { } id ? table.AddUser(id, true) : throw new ArgumentException("no bot available");
  }
}
