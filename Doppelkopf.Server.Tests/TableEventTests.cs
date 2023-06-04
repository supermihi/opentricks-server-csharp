using System.Text.Json;
using Doppelkopf.API;
using Doppelkopf.Server.TableActions;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Tests;

public class TableEventTests
{
  [Fact]
  public void CanDeserialize()
  {
    var action = JsonSerializer.Deserialize<TableEvent>("""{"type":  "cardPlayed"}""", JsonConfiguration.Options);
  }
}
