using System.Net;
using ForwardAuthGateway.Exceptions;

namespace ForwardAuthGateway.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning(ex, ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception caught in ExceptionHandlingMiddleware");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}