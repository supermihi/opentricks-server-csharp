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
    Console.WriteLine("these are the tables");
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
    _client.SubscribeToUpdates();
    while (true)
    {
      Console.WriteLine($"table: {_table!.Name}");
      var what = Prompt.Select("what to do?", new[] { "get state", "add bot", "join", "start", "quit" });
      switch (what)
      {
        case "get state":
          _table = await _client.GetTable(_table.Id);
          PrintTable();
          break;
        case "add bot":
          var addBotResponse = await _client.Act(_table.Id, new TableRequest(RequestType.AddBot));
          Console.WriteLine(addBotResponse);
          _table = addBotResponse;
          break;
        case "join":
          throw new NotImplementedException();
        case "start":
          var response = await _client.Act(_table.Id, new TableRequest(RequestType.Start));
          Console.WriteLine(response);
          _table = response;
          await PlayLoop();
          break;
        case "play":
          await PlayLoop();
          break;
        case "quit":
          return;
      }
    }
  }

  private async Task PlayLoop()
  {
    while (true)
    {
      Console.WriteLine($"table: {_table!.Name}");
      PrintTable();
      var what = Prompt.Select("what to do?", new[] { "gesund", "card", "quit" });
      switch (what)
      {
        case "gesund":
          _table = await _client.Act(_table.Id, new TableRequest(RequestType.Reserve) { IsReserved = false });
          continue;
        case "card":
          var mySeat = _table.Players.ToList().IndexOf(_client.User);
          var myPlayer = _table.Session!.PlayerIndexes.ToList().IndexOf(mySeat);
          var cards = _table.Session.CurrentGame!.Cards[myPlayer];
          var cardsWithIndex = cards.Select((c, i) => $"{i}: {c}").ToList();
          var cardWithIndex = Prompt.Select("which card", cardsWithIndex);
          var card = cards[cardsWithIndex.IndexOf(cardWithIndex)];
          _table = await _client.Act(_table.Id, new TableRequest(RequestType.PlayCard) { CardId = card });
          continue;
        case "quit":
          return;
      }
    }
  }

  private void PrintTable() => Console.WriteLine(JsonSerializer.Serialize(_table, JsonConfiguration.Pretty));
}
