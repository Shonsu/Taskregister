using Microsoft.AspNetCore.Mvc;

namespace Taskregister.Server.Shared;

public static class ResultExtension
{
    public static ObjectResult Match<T>(this Result<T> result,
        Func<T, ObjectResult> onSuccess,
        Func<Error, ObjectResult> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }
    
    // public static ActionResult Match<T>(this Result<T> result,
    //         Func<NoContentResult> onSuccess,
    //         Func<Error, ObjectResult> onFailure)
    //     {
    //         return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    //     }
}
