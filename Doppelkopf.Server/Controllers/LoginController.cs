using Doppelkopf.API;
using Doppelkopf.Server.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Controllers;

[ApiController]
[Route("/login")]
[AllowAnonymous]
public class LoginController : ControllerBase
{
  private readonly ILoginHandler _loginHandler;

  public LoginController(ILoginHandler loginHandler)
  {
    _loginHandler = loginHandler;
  }

  [HttpPost]
  public async Task Login(LoginRequest request)
  {
    if (await _loginHandler.Login(new(request.Id), request.Secret, request.Name) is { } user)
    {
      UserCookie.Set(Response.Cookies, user.Id, user.Name, request.Secret);
    }
  }
}
