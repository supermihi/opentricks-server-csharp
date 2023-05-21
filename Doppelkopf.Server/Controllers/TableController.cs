using Doppelkopf.Cards;
using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Controllers.Interface;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Controllers;

[ApiController]
[Route("table")]
[Authorize]
public class TableController : ControllerBase
{
  private readonly ILogger<TableController> _logger;
  private readonly ITableProvider _tableProvider;

  public TableController(ILogger<TableController> logger, ITableProvider tableProvider)
  {
    _logger = logger;
    _tableProvider = tableProvider;
  }

  [HttpGet(Name = "GetTables")]
  public IAsyncEnumerable<ITable> GetAsync()
  {
    return _tableProvider.GetAll();
  }

  [HttpPost(Name = "CreateTable")]
  public async Task<JsonTable> CreateAsync(JsonCreateTableRequest request)
  {
    var table = await _tableProvider.Create(HttpContext.AuthenticatedUserId(), request.Name);
    return JsonTable.FromTableData(table.Data);
  }

  [HttpGet("{id}", Name = "GetTable")]
  public async Task<ITable> GetTableAsync(string id)
  {
    var table = await _tableProvider.Get(new TableId(id));
    if (!table.Data.Users.Contains(HttpContext.AuthenticatedUserId()))
    {
      throw new Exception("forbidden");
    }
    return table;
  }

  [HttpPost("{id}/playCard", Name = "PlayCard")]
  public async Task PlayCardAsync(
    string id,
    [FromHeader(Name = "User")] string user,
    [FromBody] Card card
  )
  {
    var table = await _tableProvider.Get(new TableId(id));
    await table.PlayCard(new UserId(user), card);
  }
}
