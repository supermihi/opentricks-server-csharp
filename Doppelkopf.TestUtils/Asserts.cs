using Doppelkopf.Errors;
using Xunit;

namespace Doppelkopf.TestUtils;

public static class Asserts
{
  public static void ThrowsErrorCode(ErrorCode error, Action action)
  {
    var exception = Assert.Throws<InvalidMoveException>(action);
    Assert.Equal(error.Code, exception.Code);
  }
}
