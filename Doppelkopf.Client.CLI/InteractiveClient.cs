using System.Text.Json;
using Doppelkopf.API;
using Sharprompt;

namespace Doppelkopf.Client.CLI;

public class InteractiveClient
{
  private readonly DoppelkopfClient _client;
  private TableState? _table;

  public InteractiveClient(DoppelkopfClient client)
  {
    _client = client;
  }

  public async Task Run()
  {
    await _client.Login();
    while (true)
    {
      var action = Prompt.Select("What to do?", new[] { "create table", "list tables", "quit" });
      switch (action)
      {
        case "create table":
          await CreateTable();
          break;
        case "list tables":
          await ListTables();
          break;
        case "quit":
          return;
      }
    }
  }

  private async Task CreateTable()
  {
    var name = Prompt.Input<string>("name (default: 'unnamed')?", defaultValue: "unnamed");
    var table = await _client.CreateTable(new(name, RuleSet.Minikopf));
    Console.WriteLine($"created table {table}");
    _table = table;
    await TableLoop();
  }

  private async Task ListTables()
  {
    var tables = await _client.ListTables();
    Console.WriteLine($"these are the tables");
    foreach (var table in tables)
    {
      Console.WriteLine(table);
    }
    if (tables.Count > 0)
    {
      var selectedTable = Prompt.Select("select table to use", tables.Select(t => t.Id));
      _table = tables.Single(t => t.Id == selectedTable);
      await TableLoop();
    }
  }

  private async Task TableLoop()
  {
    while (true)
    {
      Console.WriteLine($"table: {_table!.Name}");
      var what = Prompt.Select("what to do?", new[] { "get state", "join", "start", "quit" });
      switch (what)
      {
        case "get state":
          var table = await _client.GetTable(_table.Id);
          Console.WriteLine($"current state: {table}");
          break;
        case "join":
          throw new NotImplementedException();
        case "start":
          var response = await _client.Act(_table.Id, new TableRequest(RequestType.Start));
          Console.WriteLine(response);
          break;
        case "quit":
          return;
      }
    }
  }
}
