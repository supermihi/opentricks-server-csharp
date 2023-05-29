using System.Text.Json;
using Doppelkopf.Server.Interface;
using Doppelkopf.Server.TableActions;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Tests;

public class TableActionTests
{
  [Fact]
  public void CanDeserialize()
  {
    var options = new JsonOptions();
    JsonConfiguration.SetupJsonOptions(options);
    var action = JsonSerializer.Deserialize<TableEvent>("""{"type":  "playCard"}""", options.JsonSerializerOptions);
  }
}
