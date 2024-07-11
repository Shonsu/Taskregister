using Microsoft.AspNetCore.Diagnostics;

namespace Taskregister.Server.ExceptionHandler;

public class GlobalExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}