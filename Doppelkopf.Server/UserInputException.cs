using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Doppelkopf.Server;

public class UserInputException : Exception
{
  public HttpStatusCode StatusCode { get; }
  public string ErrorCode { get; }
  public string Description { get; }

  public UserInputException(HttpStatusCode statusCode, string errorCode, string description)
  {
    StatusCode = statusCode;
    ErrorCode = errorCode;
    Description = description;
  }
}

public class MyExceptionFilter : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    if (context.Exception is UserInputException uie)
    {
      context.Result =
          new JsonResult(
            new Dictionary<string, object>() { ["code"] = uie.ErrorCode, ["description"] = uie.Description })
          {
            StatusCode = (int)uie.StatusCode
          };
      context.ExceptionHandled = true;
    }
  }
}
