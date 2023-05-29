using Doppelkopf.Server.Authentication;
using Doppelkopf.Server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Controllers;

[ApiController]
[Route("/user")]
[Authorize]
public class UserController : ControllerBase
{

  [HttpGet]
  public Task<UserData> Index()
  {
    return Task.FromResult(HttpContext.AuthenticatedUser());
  }

}
