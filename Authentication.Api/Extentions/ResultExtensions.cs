using Authentication.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            return new ObjectResult(result.Value)
            {
                StatusCode = successStatusCode
            };
        }

        var error = result.Error!;

        // Prefer an explicit status code from the domain error when available
        if (error.StatusCode.HasValue)
        {
            var status = error.StatusCode.Value;

            return controller.StatusCode(
                status,
                new ProblemDetails
                {
                    Status = status,
                    Title = "Request failed",
                    Detail = error.Message
                });
        }

        return controller.BadRequest(
                new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Request failed",
                    Detail = "Internal server error"
                });   
    }
}