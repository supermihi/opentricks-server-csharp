using System.Security.Claims;
using System.Text.Json;
using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Controllers;
using Doppelkopf.Server.Interface;
using Doppelkopf.Server.Model;
using Doppelkopf.Server.Notifications;
using Doppelkopf.Server.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Doppelkopf.Server.Tests;

public class CreateTableTests
{
  [Fact]
  public async Task SucceedsWithOnlyName()
  {
    var controller = CreateController();
    await controller.CreateAsync(JsonSerializer.Deserialize<JsonCreateTableRequest>("""{"name":"test"}""")!);
  }

  [Fact]
  public async Task FailsWithoutName()
  {
    var controller = CreateController();
    await controller.CreateAsync(JsonSerializer.Deserialize<JsonCreateTableRequest>("""{}""")!);
  }

  [Fact]
  public async Task FailsWithUnknownField()
  {
    var controller = CreateController();
    await controller.CreateAsync(JsonSerializer.Deserialize<JsonCreateTableRequest>("""{"bla":  "blub"}""")!);
  }

  private static TableController CreateController() {
    var controller = new TableController(
      Mock.Of<ILogger<TableController>>(),
      Mock.Of<ITableStore>(),
      Mock.Of<ITableActionListener>()) {
      ControllerContext = {
        HttpContext = new DefaultHttpContext {
          User = Claims.CreateClaimsPrincipal(new UserData(new("m"), "michi"), "test")
        }
      }
    };
    return controller;
  }
}