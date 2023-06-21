namespace Doppelkopf.Server.Tests;

public static class MyAssert
{
  public static UserInputException ThrowsUserError(string code, Action action)
  {
    var exception = Assert.Throws<UserInputException>(action);
    Assert.Equal(code, exception.ErrorCode);
    return exception;
  }

  public static async Task<UserInputException> ThrowsUserErrorAsync(string code, Func<Task> action)
  {
    var exception = await Assert.ThrowsAsync<UserInputException>(action);
    Assert.Equal(code, exception.ErrorCode);
    return exception;
  }
}
