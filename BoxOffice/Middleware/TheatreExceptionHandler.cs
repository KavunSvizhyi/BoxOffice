using BoxOffice.Business.Validation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace BoxOffice.Middleware
{
    public class TheatreExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                TheatreException => (StatusCodes.Status400BadRequest, "Bad data"),
                _ => (StatusCodes.Status500InternalServerError, "Something on the server side")
            };

            var problemDetails = new ProblemDetails()
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = httpContext.Request.Path,
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
