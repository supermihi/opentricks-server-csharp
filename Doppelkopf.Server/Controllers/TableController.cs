using Doppelkopf.Cards;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Controllers;

[ApiController]
[Route("table")]
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
  public async Task<ITable> CreateAsync()
  {
    var table = await _tableProvider.Create(CurrentUserId);
    return table;
  }

  private UserId CurrentUserId => new("");

  [HttpGet("{id}", Name = "GetTable")]
  public async Task<ITable> GetTableAsync(string id)
  {
    var table = await _tableProvider.Get(new TableId(id));
    if (!table.Meta.ContainsUser(CurrentUserId))
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
