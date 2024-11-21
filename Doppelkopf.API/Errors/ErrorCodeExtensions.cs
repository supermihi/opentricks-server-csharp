using System.Diagnostics.CodeAnalysis;

namespace Doppelkopf.API.Errors;

public static class ErrorCodeExtensions
{
    [DoesNotReturn]
    public static void Throw(this ErrorCode code)
    {
        throw code.ToException();
    }

    public static InvalidMoveException ToException(this ErrorCode code)
    {
        return new(code);
    }
}
